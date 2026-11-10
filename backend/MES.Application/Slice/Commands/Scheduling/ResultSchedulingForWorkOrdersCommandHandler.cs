namespace MesMicroservice.Application.Commands.Scheduling
{
    public class ResultSchedulingForWorkOrdersCommandHandler : IRequestHandler<ResultSchedulingForWorkOrdersCommand, Dictionary<string, object>>
    {
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IMediator _mediator;

        public ResultSchedulingForWorkOrdersCommandHandler(IEquipmentRepository equipmentRepository, IWorkOrderRepository workOrderRepository, IMediator mediator)
        {
            _equipmentRepository = equipmentRepository;
            _workOrderRepository = workOrderRepository;
            _mediator = mediator;
        }

        public async Task<Dictionary<string, object>> Handle(ResultSchedulingForWorkOrdersCommand request, CancellationToken cancellationToken)
        {
            // Múi giờ Việt Nam
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Lấy thời gian hiện tại theo múi giờ Việt Nam
            var utcNow = DateTime.UtcNow;
            var nowVN = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vnTimeZone);

            // Đặt mặc định bắt đầu là 8h sáng ngày hôm sau
            var defaultStartVN = new DateTime(
                nowVN.Year,
                nowVN.Month,
                nowVN.Day,
                nowVN.Hour,
                nowVN.Minute,
                nowVN.Second,      
                DateTimeKind.Unspecified
            );

            // Nếu bạn muốn lưu vào DB dạng UTC, có thể convert:
            var defaultStartUtc = TimeZoneInfo.ConvertTimeToUtc(defaultStartVN, vnTimeZone);

            var schedulingCommand = new SchedulingForWorkOrdersCommand(
                request.WorkOrderIds,
                request.AlgorithmType,
                request.UrgentWorkOrderIds
            );
            var solution = await _mediator.Send(schedulingCommand, cancellationToken);
            
            var scheduledWorkOrders = new List<ScheduledWorkOrderDTO>();

            if (solution?.BestSchedule?.Jobs != null)
            {
                foreach (var moJobs in solution.BestSchedule.Jobs)
                {
                    string manufacturingOrderId = moJobs.Key;
                    var jobs = moJobs.Value;

                    foreach (var job in jobs)
                    {
                        // Convert DueDate từ ticks sang DateTime UTC → rồi sang giờ Việt Nam (nếu cần hiển thị)
                        var dueDateUtc = new DateTime((long)job.DueDate, DateTimeKind.Utc);
                        var dueDateVN = TimeZoneInfo.ConvertTimeFromUtc(dueDateUtc, vnTimeZone);

                        // Tính toán start và end dựa trên mốc mặc định 8h sáng hôm sau (giờ VN)
                        var startVN = defaultStartVN.AddHours(job.StartTime);
                        var endVN = defaultStartVN.AddHours(job.EndTime);

                        // Nếu cần lưu vào DB theo UTC:
                        var startUtc = TimeZoneInfo.ConvertTimeToUtc(startVN, vnTimeZone);
                        var endUtc = TimeZoneInfo.ConvertTimeToUtc(endVN, vnTimeZone);

                        // Lấy thông tin từ repository
                        var workOrder = await _workOrderRepository.GetById(job.Id);
                        var equipment = await _equipmentRepository.GetAsync(job.AssignedMachineCode ??throw new Exception($"Not Found {job.AssignedMachineCode}"));

                        // Tạo DTO
                        var dto = new ScheduledWorkOrderDTO(
                            workOrderId: workOrder.WorkOrderId,
                            segmentId: workOrder?.SegmentId ?? string.Empty,
                            workCenter: equipment?.HierarchyModel?.Name ?? "",
                            sequenceNumber: job.SequenceNumber,
                            materialDefinition: workOrder?.MaterialDefinition?.ResourceId ?? string.Empty,
                            productionTarget: (decimal)(job.Quatity > 0 ? job.Quatity : 0),
                            unit: workOrder?.Unit ?? "None",
                            dueDate: workOrder.DueDate,
                            duration: job.TotalTime,
                            startTime: startVN,
                            endTime: endVN,
                            actuallyStartTime: null,
                            actuallyEndTime: null,
                            workOrderStatus: EStatus.ReadyForProduction,
                            qualityStandardId: workOrder?.QualityStandardId ?? string.Empty,
                            setupTime: job.SetupTime,
                            runTime: job.ProcessingTime,
                            manufacturingOrderId: workOrder?.ManufacturingOrder.ManufacturingOrderId ?? "", // Có thể cập nhật nếu mapping từ DB
                            equipmentId: equipment?.ResourceId ?? "",
                            personShift12: job.Employees.FirstOrDefault(x => x.shift1 is true)?.PersonId ?? "",
                            personShift3: job.Employees.FirstOrDefault(x => x.shift3 is true)?.PersonId ?? "",
                            productionParameterId: workOrder?.OperationsSegment.ProductionParameterId ?? string.Empty
                        );

                        var wo = await _workOrderRepository.GetListBomByWoIdAsync(workOrder.WorkOrderId);

                        var bomIds = new List<string>();
                        foreach (var item in wo.OperationsSegment.OperationsDefinition.OperationsMaterialBill.Items)
                        {
                            foreach (var materialSpecification in workOrder.OperationsSegment.MaterialSpecifications)
                            {
                                if (item.MaterialDefinitionId == materialSpecification.MaterialDefinitionId)
                                {
                                    bomIds.Add(item.BillId);
                                }
                            }
                        }

                        dto.BOMs = bomIds;

                        scheduledWorkOrders.Add(dto);
                    }
                }
            }

            // Gom WO theo manufacturingOrderId, tạo 1 Dictionary key là MO ID, value là list WO
            ValidateNoMachineOverlap(scheduledWorkOrders);
            var WODict = new Dictionary<string, object>();
            foreach (var swo in scheduledWorkOrders.OrderBy(x => x.StartTime))
            {
                if (!WODict.ContainsKey(swo.ManufacturingOrderId))
                {
                    WODict[swo.ManufacturingOrderId] = new List<ScheduledWorkOrderDTO>();
                }

                ((List<ScheduledWorkOrderDTO>)WODict[swo.ManufacturingOrderId]).Add(swo);
            }

            return WODict;
        }
        private static void ValidateNoMachineOverlap(List<ScheduledWorkOrderDTO> scheduledWorkOrders)
        {
            var rows = scheduledWorkOrders
                .Where(x => !string.IsNullOrWhiteSpace(x.EquipmentId))
                .GroupBy(x => x.EquipmentId);

            foreach (var machineGroup in rows)
            {
                var ordered = machineGroup
                    .OrderBy(x => x.StartTime)
                    .ToList();

                for (int i = 1; i < ordered.Count; i++)
                {
                    var previous = ordered[i - 1];
                    var current = ordered[i];

                    if (current.StartTime < previous.EndTime)
                    {
                        throw new InvalidOperationException(
                            $"Lịch không hợp lệ: máy {machineGroup.Key} bị trùng thời gian giữa " +
                            $"{previous.WorkOrderId} ({previous.StartTime:yyyy-MM-dd HH:mm} - {previous.EndTime:yyyy-MM-dd HH:mm}) và " +
                            $"{current.WorkOrderId} ({current.StartTime:yyyy-MM-dd HH:mm} - {current.EndTime:yyyy-MM-dd HH:mm})."
                        );
                    }
                }
            }
        }

    }
}
