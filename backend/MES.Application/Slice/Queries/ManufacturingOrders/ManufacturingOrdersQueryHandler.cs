using MesMicroservice.Domain.AggregateModels.ManufacturingOrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace MesMicroservice.Application.Queries.ManufacturingOrders;

public class ManufacturingOrdersQueryHandler : IRequestHandler<ManufacturingOrdersQuery, IEnumrable<ManufacturingOrderDTO>>
{
    private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
    private readonly IMapper _mapper;

    public ManufacturingOrdersQueryHandler(IManufacturingOrderRepository manufacturingOrderRepository, IMapper mapper)
    {
        _manufacturingOrderRepository = manufacturingOrderRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<ManufacturingOrderDTO>> Handle(ManufacturingOrdersQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _manufacturingOrderRepository.GetManufacturingOrders();

        if (request.IdStartedWith is not null)
        {
            queryable = queryable.Where(x => x.ManufacturingOrderId.StartsWith(request.IdStartedWith));
        }

        if (request.Paginated)
        {
            queryable = queryable
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        int totalItems = queryable.Count();

        var manufacturingOrders = queryable.ToList();
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

        var queryResult = new IEnumrable<ManufacturingOrderDTO>(manufacturingOrderDTOs, totalItems);

        return queryResult;
    }
}
