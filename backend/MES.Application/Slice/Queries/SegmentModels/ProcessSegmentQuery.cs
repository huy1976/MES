namespace MesMicroservice.Application.Queries.SegmentModels;

public class ProcessSegmentQuery : IRequest<ProcessSegmentViewModel?>
{
    public string ProcessSegmentId { get; set; }

    public ProcessSegmentQuery(string processSegmentId)
    {
        ProcessSegmentId = processSegmentId;
    }
}
