namespace MesMicroservice.Application.Queries.BillOfMaterials;

public class BillOfMaterialsQuery : PaginatedQuery, IRequest<IEnumrable<BillOfMaterialViewModel>>
{
    public string? IdStartedWith { get; set; }
}
