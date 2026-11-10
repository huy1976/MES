
namespace MesMicroservice.Application.Queries.ProductionParameters;

public class ProductionParameterPropertyNamesQueryHandler : IRequestHandler<ProductionParameterPropertyNamesQuery, IEnumerable<string>>
{
    private readonly IProductionParameterRepository _productionParameterRepository;
    public ProductionParameterPropertyNamesQueryHandler(IProductionParameterRepository productionParameterRepository)
    {
        _productionParameterRepository = productionParameterRepository;
    }
    public async Task<IEnumerable<string>> Handle(ProductionParameterPropertyNamesQuery request, CancellationToken cancellationToken)
    {
        var productionParameters = await _productionParameterRepository.GetProductionParameters();

        var propertyNames = productionParameters
            .SelectMany(pp => pp.Properties)
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        return propertyNames;
    }
}
