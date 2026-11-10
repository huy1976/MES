namespace MesMicroservice.Application.Queries.Enterprises;
public class WorkUnitsQuery : PaginatedQuery, IRequest<IEnumrable<WorkUnitViewModel>>
{
    public string? IdStartedWith { get; set; }
}
