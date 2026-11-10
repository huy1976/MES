namespace MesMicroservice.Application.Commands.WorkOrders
{
    public class AddListWorkOrdersCommandHandler : IRequestHandler<AddListWorkOrdersCommand, bool>
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
        private readonly IProcessSegmentRepository _processSegmentRepository;
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
        public AddListWorkOrdersCommandHandler(IWorkOrderRepository workOrderRepository, IManufacturingOrderRepository manufacturingOrderRepository, IProcessSegmentRepository processSegmentRepository, IMaterialDefinitionRepository materialDefinitionRepository)
        {
            _workOrderRepository = workOrderRepository;
            _manufacturingOrderRepository = manufacturingOrderRepository;
            _processSegmentRepository = processSegmentRepository;
            _materialDefinitionRepository = materialDefinitionRepository;
        }

        public async Task<bool> Handle(AddListWorkOrdersCommand request, CancellationToken cancellationToken)
        {

            var manufacturingOrders = new List<ManufacturingOrder>();
            foreach (var workOrderDTO in request.workOrderDTOs)
            {
                var manufacturingOrder = await _manufacturingOrderRepository.GetAsync(workOrderDTO.ManufacturingOrderId)
                    ?? throw new ResourceNotFoundException(nameof(ManufacturingOrder), workOrderDTO.ManufacturingOrderId);

                var operationsDefinition = manufacturingOrder.OperationsDefinition;
                var operationsSegment = operationsDefinition.OperationsSegments.FirstOrDefault(os => os.Id == workOrderDTO.OperationsSegmentId)
                    ?? throw new ResourceNotFoundException(nameof(OperationsSegment), workOrderDTO.OperationsSegmentId.ToString());

                var processSegment = await _processSegmentRepository.GetAsync(workOrderDTO.SegmentId);


                var checkWorkOrderExists = await _workOrderRepository.ExistsAsync(manufacturingOrder.Id, workOrderDTO.WorkOrderId);
                if (checkWorkOrderExists)
                {
                    throw new ResourceAlreadyExistsException(nameof(WorkOrder), workOrderDTO.WorkOrderId);
                }

                var materialDefinition = await _materialDefinitionRepository.GetAsync(operationsSegment.OutputMaterialDefinitionId);
                if (materialDefinition is null)
                {
                    throw new ResourceNotFoundException(nameof(MaterialDefinition), operationsSegment.OutputMaterialDefinitionId);
                }

                var workOrder = new WorkOrder(
                    workOrderId: workOrderDTO.WorkOrderId,
                    segmentId: workOrderDTO.SegmentId,
                    operationsSegment: operationsSegment,
                    workCenter: null,
                    sequenceNumber: workOrderDTO.SequenceNumber,
                    materialDefinition: materialDefinition,
                    productionTarget: workOrderDTO.ProductionTarget,
                    unit: workOrderDTO.Unit,
                    dueDate: manufacturingOrder.DueDate,
                    duration: 0.0,
                    startTime: DateTime.MinValue,
                    endTime: DateTime.MinValue,
                    actuallyStartTime: DateTime.MinValue,
                    actuallyEndTime: DateTime.MinValue,
                    workOrderStatus: EStatus.PendingScheduling,
                    qualityStandardId: workOrderDTO.QualityParamId,
                    setupTime: 0.0,
                    teardownTime: 0.0,
                    runTime: 0.0,
                    manufacturingOrderId: manufacturingOrder.Id,
                    manufacturingOrder: manufacturingOrder,
                    manufacturingRecords: new List<ManufacturingRecord>(),
                    equipmentWorkOrders: new List<EquipmentWorkOrder>(),
                    personWorkOrders: new List<PersonWorkOrder>(),
                    materialWorkOrders: new List<MaterialWorkOrder>());

                await _workOrderRepository.AddAsync(workOrder);

                if (!manufacturingOrders.Contains(manufacturingOrder))
                {
                    manufacturingOrders.Add(manufacturingOrder);
                }
            }

            foreach (var mo in manufacturingOrders)
            {
                mo.UpdateStatus(EStatus.PendingScheduling);
            }
            

            return await _workOrderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
