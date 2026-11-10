
namespace MesMicroservice.Application.Queries.ProductionParameters;

public class ProductionParametersQueryHandler : IRequestHandler<ProductionParametersQuery, IEnumrable<ProductionParameterViewModel>>
{
    private readonly IProductionParameterRepository _productionParameterRepository;
    private readonly IMapper _mapper;

    public ProductionParametersQueryHandler(IProductionParameterRepository productionParameterRepository, IMapper mapper)
    {
        _productionParameterRepository = productionParameterRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<ProductionParameterViewModel>> Handle(ProductionParametersQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _productionParameterRepository.GetProductionParameters();
        if (request.IdStartedWith is not null)
        {
            queryable = queryable.Where(x => x.ResourceId.StartsWith(request.IdStartedWith));
        }

        if (request.Paginated)
        {
            queryable = queryable
                .OrderBy(x => x.ResourceId)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        int totalItems = queryable.Count();

        var productionParameters = queryable.ToList();

        var queryResult = new IEnumrable<ProductionParameter>(productionParameters, totalItems);

        return _mapper.Map<IEnumrable<ProductionParameter>, IEnumrable<ProductionParameterViewModel>>(queryResult);
    }
}
