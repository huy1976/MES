namespace MesMicroservice.Application.Queries.ManufacturingOrders
{
    public class ManufacturingOrdersByStatusQueryHandler : IRequestHandler<ManufacturingOrdersByStatusQuery, IEnumerable<ManufacturingOrderDTO>>
    {
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
        private readonly IMapper _mapper;

        public ManufacturingOrdersByStatusQueryHandler(IManufacturingOrderRepository manufacturingOrderRepository, IMapper mapper)
        {
            _manufacturingOrderRepository = manufacturingOrderRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ManufacturingOrderDTO>> Handle(ManufacturingOrdersByStatusQuery request, CancellationToken cancellationToken)
        {
            var manufacturingOrders = await _manufacturingOrderRepository.GetManufacturingOrders();

            // lấy theo status
            manufacturingOrders = manufacturingOrders.Where(mo => mo.Status == request.Status);

            var manufacturingOrderDTOs = new List<ManufacturingOrderDTO>();
            foreach (var mo in manufacturingOrders)
            {
                var moDTO = new ManufacturingOrderDTO(manufacturingOrderId: mo.ManufacturingOrderId,
                                                      materialDefinitionId: mo.MaterialDefinition.ResourceId,
                                                      materialDefinitionName: mo.MaterialDefinition.Name,
                                                      quantity: mo.Quantity,
                                                      unit: mo.Unit,
                                                      dueDate: mo.DueDate,
                                                      customerId: mo.Customer.ResourceId,
                                                      operationsDefinitionId: mo.OperationsDefinition.ResourceId,
                                                      qualityStandardId: mo.QualityStandard.ResourceId,
                                                      status: mo.Status.ToString(),
                                                      createdBy: mo.CreatedBy.ResourceId,
                                                      createDate: mo.CreateDate);

                manufacturingOrderDTOs.Add(moDTO);
            }

            return manufacturingOrderDTOs;
        }
    }
}
