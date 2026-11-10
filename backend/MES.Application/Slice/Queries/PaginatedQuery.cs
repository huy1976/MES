namespace MesMicroservice.Application.Queries;

public class PaginatedQuery
{
    public bool Paginated { get; set; } = true;
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
