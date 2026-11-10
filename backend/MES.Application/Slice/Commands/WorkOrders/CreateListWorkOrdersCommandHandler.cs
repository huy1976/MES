namespace MesMicroservice.Application.Commands.WorkOrders
{
    public class CreateListWorkOrdersCommandHandler : IRequestHandler<CreateListWorkOrdersCommand, List<WorkOrderDTO>>
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
        private readonly IProcessSegmentRepository _processSegmentRepository;

        public CreateListWorkOrdersCommandHandler(
            IWorkOrderRepository workOrderRepository,
            IManufacturingOrderRepository manufacturingOrderRepository,
            IProcessSegmentRepository processSegmentRepository)
        {
            _workOrderRepository = workOrderRepository;
            _manufacturingOrderRepository = manufacturingOrderRepository;
            _processSegmentRepository = processSegmentRepository;
        }

        public async Task<List<WorkOrderDTO>> Handle(CreateListWorkOrdersCommand request, CancellationToken cancellationToken)
        {

            var nextId = await _workOrderRepository.GetQuantityWorkOrders();
            while (await _workOrderRepository.ExistsAsync($"LSX{nextId}"))
            {
                nextId++;
            }

            // 🔹 Lấy ManufacturingOrder và kiểm tra trạng thái
            var manufacturingOrder = await _manufacturingOrderRepository.GetAsync(request.ManufacturingOrderId)
                ?? throw new ResourceNotFoundException(nameof(ManufacturingOrder), request.ManufacturingOrderId);

            if (manufacturingOrder.Status != EStatus.Pending)
                throw new InvalidOperationException("Cannot create WorkOrders for a ManufacturingOrder that is not in Pending status.");

            var operationsDefinition = manufacturingOrder.OperationsDefinition;

            // The number of stages is derived from data and is not limited by code.
            var segmentSequenceMap = CalculateSegmentSequenceNumbers(operationsDefinition);

            var workOrderDTOs = new List<WorkOrderDTO>();

            // 🔹 Duyệt qua các segment theo thứ tự
            var orderedSegments = operationsDefinition.OperationsSegments
                .OrderBy(s => segmentSequenceMap.GetValueOrDefault(s.SegmentId, int.MaxValue))
                .ToList();

            foreach (var segment in orderedSegments)
            {
                var processSegment = await _processSegmentRepository.GetAsync(segment.SegmentId)
                    ?? throw new ResourceNotFoundException(nameof(ProcessSegment), segment.SegmentId);

                // 🔹 Xác định BOM IDs liên quan đến segment hiện tại
                var bomIds = operationsDefinition.OperationsMaterialBill.Items
                    .Where(item => segment.MaterialSpecifications
                        .Any(spec => spec.MaterialDefinitionId == item.MaterialDefinitionId))
                    .Select(item => item.BillId)
                    .ToList();

                var sequenceNumber = segmentSequenceMap[segment.SegmentId];

                // 🔹 Tạo đối tượng WorkOrderDTO
                var workOrderDTO = new WorkOrderDTO(
                    workOrderId: $"LSX{nextId}",
                    segmentId: segment.SegmentId,
                    manufacturingOrderId: manufacturingOrder.ManufacturingOrderId,
                    operationsSegmentId: segment.Id,
                    operationsSegmentName: processSegment.Name,
                    materialDefinitionId: segment.OutputMaterialDefinitionId,
                    productionTarget: manufacturingOrder.Quantity,
                    unit: manufacturingOrder.Unit,
                    productionParamId: segment.ProductionParameterId,
                    qualityParamId: manufacturingOrder.QualityStandard.ResourceId,
                    bOMIds: bomIds,
                    sequenceNumber: sequenceNumber
                );

                workOrderDTOs.Add(workOrderDTO);

                nextId++;
                while (await _workOrderRepository.ExistsAsync($"LSX{nextId}"))
                {
                    nextId++;
                }
            }

            return workOrderDTOs;
        }


        /// <summary>
        /// Calculates an unlimited number of stage levels from the dependency graph.
        /// </summary>
        private Dictionary<string, int> CalculateSegmentSequenceNumbers(OperationsDefinition operationsDefinition)
        {
            var dependencies = operationsDefinition.OperationsSegmentDependencies
                .Where(d => d.IsActive)
                .Select(d => new KeyValuePair<string, string>(
                    d.FromSegmentId,
                    d.ToSegmentId));

            return MesMicroservice.Application.Services.SegmentSequencePlanner.Calculate(
                operationsDefinition.OperationsSegments.Select(s => s.SegmentId),
                dependencies);
        }
    }
}
