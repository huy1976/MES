namespace MesMicroservice.Application.Queries.SegmentModels;

public class ProcessSegmentsQuery : PaginatedQuery, IRequest<IEnumrable<ProcessSegmentViewModel>>
{
    public string? IdStartedWith { get; set; }
}
