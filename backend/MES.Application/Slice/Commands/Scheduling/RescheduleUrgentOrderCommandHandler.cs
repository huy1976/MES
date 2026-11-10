namespace MesMicroservice.Application.Commands.Scheduling
{
    public class RescheduleUrgentOrderCommandHandler
        : IRequestHandler<RescheduleUrgentOrderCommand, Dictionary<string, object>>
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IMediator _mediator;

        public RescheduleUrgentOrderCommandHandler(
            IWorkOrderRepository workOrderRepository,
            IMediator mediator)
        {
            _workOrderRepository = workOrderRepository;
            _mediator = mediator;
        }

        public async Task<Dictionary<string, object>> Handle(
            RescheduleUrgentOrderCommand request,
            CancellationToken cancellationToken)
        {
            if (request.UrgentWorkOrderIds == null || !request.UrgentWorkOrderIds.Any())
                throw new InvalidOperationException("Vui lòng chọn ít nhất một lệnh sản xuất quan trọng.");

            var urgentIds = request.UrgentWorkOrderIds.Distinct().ToList();
            var urgentWorkOrders = await _workOrderRepository.GetByIds(urgentIds);

            if (urgentWorkOrders.Count != urgentIds.Count)
                throw new InvalidOperationException("Có lệnh sản xuất quan trọng không tồn tại.");

            var invalidUrgent = urgentWorkOrders
                .Where(wo => IsLockedStatus(wo.WorkOrderStatus))
                .ToList();

            if (invalidUrgent.Any())
            {
                var invalidCodes = string.Join(", ", invalidUrgent.Select(wo => wo.WorkOrderId));
                throw new InvalidOperationException(
                    $"Không thể chen các lệnh đang chạy, đã hoàn thành hoặc đã hủy: {invalidCodes}");
            }

            var allWorkOrders = await _workOrderRepository.GetAll();

            var urgentMoCodes = urgentWorkOrders
                .Select(wo => wo.ManufacturingOrder?.ManufacturingOrderId)
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var urgentMoWorkOrders = allWorkOrders
                .Where(wo => urgentMoCodes.Contains(wo.ManufacturingOrder?.ManufacturingOrderId ?? ""))
                .Where(wo => IsCandidateStatus(wo.WorkOrderStatus))
                .ToList();

            List<WorkOrder> reschedulableWorkOrders;

            if (request.ReschedulableWorkOrderIds != null && request.ReschedulableWorkOrderIds.Any())
            {
                reschedulableWorkOrders = await _workOrderRepository.GetByIds(
                    request.ReschedulableWorkOrderIds.Distinct().ToList()
                );

                var invalidReschedule = reschedulableWorkOrders
                    .Where(wo => IsLockedStatus(wo.WorkOrderStatus))
                    .ToList();

                if (invalidReschedule.Any())
                {
                    var invalidCodes = string.Join(", ", invalidReschedule.Select(wo => wo.WorkOrderId));
                    throw new InvalidOperationException(
                        $"Không thể điều độ lại các lệnh đang chạy, đã hoàn thành hoặc đã hủy: {invalidCodes}");
                }
            }
            else
            {
                var latestUrgentDueDate = urgentWorkOrders.Max(wo => wo.DueDate);
                var horizonEnd = latestUrgentDueDate.AddDays(7);

                // Không lấy toàn bộ PendingScheduling nữa.
                // Chỉ lấy các lệnh đã nằm trong lịch kế hoạch và chưa chạy.
                reschedulableWorkOrders = allWorkOrders
                    .Where(IsExistingScheduledPlan)
                    .Where(wo => wo.StartTime <= horizonEnd)
                    .ToList();
            }

            var candidateWorkOrders = urgentWorkOrders
                .Concat(urgentMoWorkOrders)
                .Concat(reschedulableWorkOrders)
                .Where(wo => IsCandidateStatus(wo.WorkOrderStatus))
                .DistinctBy(wo => wo.Id)
                .ToList();

            if (candidateWorkOrders.Count < 2)
                throw new InvalidOperationException("Cần ít nhất 2 lệnh sản xuất để điều độ lại.");

            var maxCandidateCount = request.AlgorithmType == EAlgorithmType.BruteForce ? 10 : 80;
            if (candidateWorkOrders.Count > maxCandidateCount)
            {
                throw new InvalidOperationException(
                    $"Số lệnh cần điều độ lại quá lớn ({candidateWorkOrders.Count} lệnh). " +
                    $"Vui lòng thu hẹp phạm vi lịch cần điều độ lại. Giới hạn hiện tại là {maxCandidateCount} lệnh."
                );
            }

            var urgentCandidateWorkOrderIds = candidateWorkOrders
                .Where(wo => urgentMoCodes.Contains(wo.ManufacturingOrder?.ManufacturingOrderId ?? ""))
                .Select(wo => wo.Id)
                .Distinct()
                .ToList();

            Console.WriteLine(
                $"[Urgent Reschedule] Urgent={urgentWorkOrders.Count}, " +
                $"UrgentMO={urgentMoWorkOrders.Count}, " +
                $"Reschedulable={reschedulableWorkOrders.Count}, " +
                $"Candidates={candidateWorkOrders.Count}, " +
                $"Algorithm={request.AlgorithmType}"
            );

            var schedulingCommand = new ResultSchedulingForWorkOrdersCommand(
                candidateWorkOrders.Select(wo => wo.Id).Distinct().ToList(),
                request.AlgorithmType,
                urgentCandidateWorkOrderIds
            );

            return await _mediator.Send(schedulingCommand, cancellationToken);
        }

        private static bool IsCandidateStatus(EStatus status)
        {
            return status == EStatus.PendingScheduling
                || status == EStatus.Planned
                || status == EStatus.ReadyForProduction;
        }

        private static bool IsExistingScheduledPlan(WorkOrder workOrder)
        {
            return (workOrder.WorkOrderStatus == EStatus.Planned
                    || workOrder.WorkOrderStatus == EStatus.ReadyForProduction)
                && workOrder.StartTime.HasValue
                && workOrder.EndTime.HasValue;
        }

        private static bool IsLockedStatus(EStatus status)
        {
            return status == EStatus.InProduction
                || status == EStatus.Completed
                || status == EStatus.Cancelled;
        }
    }
}