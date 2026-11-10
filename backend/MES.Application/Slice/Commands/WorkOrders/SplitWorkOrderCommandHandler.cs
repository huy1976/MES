namespace MesMicroservice.Application.Commands.WorkOrders
{
    public class SplitWorkOrderCommandHandler : IRequestHandler<SplitWorkOrderCommand, bool>
    {
        private readonly IWorkOrderRepository _workOrderRepository;

        public SplitWorkOrderCommandHandler(IWorkOrderRepository workOrderRepository)
        {
            _workOrderRepository = workOrderRepository;
        }

        public async Task<bool> Handle(SplitWorkOrderCommand request, CancellationToken cancellationToken)
        {
            // 🔹 Lấy WorkOrder gốc
            var parentWO = await _workOrderRepository.GetById(request.WorkOrderId)
                ?? throw new ResourceNotFoundException(nameof(WorkOrder), request.WorkOrderId.ToString());

            if (parentWO.ProductionTarget <= request.SplitQuantity || request.SplitQuantity <= 0)
                throw new InvalidOperationException("Split quantity must be less than current production target and greater than 0.");

            if (parentWO.WorkOrderStatus != EStatus.PendingScheduling)
                throw new InvalidOperationException("Only PendingScheduling WorkOrder can be split.");

            var originalQuantity = parentWO.ProductionTarget;
            var newParentQuantity = originalQuantity - request.SplitQuantity;

            // 🔹 Update lại WorkOrder cha
            parentWO.UpdateProductionTarget(newParentQuantity);

            // 🔹 Tạo mã WorkOrder mới con
            var nextId = await _workOrderRepository.GetQuantityWorkOrders();
            while (await _workOrderRepository.ExistsAsync($"LSX{nextId}"))
            {
                nextId++;
            }

            var newWorkOrderId = $"LSX{nextId}";

            // ✅ Tạo WorkOrder con
            var childWO = new WorkOrder(
                workOrderId: newWorkOrderId,
                segmentId: parentWO.SegmentId,
                operationsSegment: parentWO.OperationsSegment,
                workCenter: null,
                sequenceNumber: parentWO.SequenceNumber,
                materialDefinition: parentWO.MaterialDefinition,
                productionTarget: request.SplitQuantity,
                unit: parentWO.Unit,
                dueDate: parentWO.DueDate,
                duration: 0.0,
                startTime: DateTime.MinValue,
                endTime: DateTime.MinValue,
                actuallyStartTime: DateTime.MinValue,
                actuallyEndTime: DateTime.MinValue,
                workOrderStatus: EStatus.PendingScheduling,
                qualityStandardId: parentWO.QualityStandardId,
                setupTime: 0.0,
                teardownTime: 0.0,
                runTime: 0.0,
                manufacturingOrderId: parentWO.ManufacturingOrderId,
                manufacturingOrder: parentWO.ManufacturingOrder,
                manufacturingRecords: new List<ManufacturingRecord>(),
                equipmentWorkOrders: new List<EquipmentWorkOrder>(),
                personWorkOrders: new List<PersonWorkOrder>(),
                materialWorkOrders: new List<MaterialWorkOrder>()
            );

            // ✅ Thêm vào MO
            parentWO.ManufacturingOrder.AddWorkOrder(childWO);

            // 🔹 Cập nhật lại trạng thái WorkOrders
            parentWO.UpdateWorkOrderStatus(EStatus.PendingScheduling);
            childWO.UpdateWorkOrderStatus(EStatus.PendingScheduling);

            return await _workOrderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
