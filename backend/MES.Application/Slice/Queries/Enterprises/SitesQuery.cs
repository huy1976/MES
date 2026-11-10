namespace MesMicroservice.Application.Queries.Enterprises;

public class SitesQuery : PaginatedQuery, IRequest<IEnumrable<SiteViewModel>>
{
    public string? IdStartedWith { get; set; }
}
