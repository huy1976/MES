namespace MesMicroservice.Application.Queries.MaterialLots;

public class MaterialLotsQuery : PaginatedQuery, IRequest<IEnumrable<MaterialLotViewModel>>
{
    public string? IdStartedWith { get; set; }
    public string? MaterialDefinitionId { get; set; }
}
