namespace MesMicroservice.Application.Queries.ProductionParameters;

public class ProductionParametersQuery : PaginatedQuery, IRequest<IEnumrable<ProductionParameterViewModel>>
{
    public string? IdStartedWith { get; set; }
}
