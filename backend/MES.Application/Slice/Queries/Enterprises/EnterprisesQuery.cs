namespace MesMicroservice.Application.Queries.Enterprises;

public class EnterprisesQuery : PaginatedQuery, IRequest<IEnumrable<EnterpriseViewModel>>
{
    public string? IdStartedWith { get; set; }
}
