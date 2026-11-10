
namespace MesMicroservice.Application.Queries.ProductionParameters;

public class ProductionParameterIdsQueryHandler : IRequestHandler<ProductionParameterIdsQuery, IEnumerable<string>>
{
    private readonly IProductionParameterRepository _productionParameterRepository;
    public ProductionParameterIdsQueryHandler(IProductionParameterRepository productionParameterRepository)
    {
        _productionParameterRepository = productionParameterRepository;
    }
    public async Task<IEnumerable<string>> Handle(ProductionParameterIdsQuery request, CancellationToken cancellationToken)
    {
        var productionParameters = await _productionParameterRepository.GetProductionParameters();

        return productionParameters.Select(x => x.ResourceId);
    }
}
