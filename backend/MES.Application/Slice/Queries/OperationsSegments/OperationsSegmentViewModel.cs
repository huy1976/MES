namespace MesMicroservice.Application.Queries.OperationsSegments;

public class OperationsSegmentViewModel
{
    public string SegmentId { get; private set; }
    public string? Description { get; set; }

    // [ĐÃ ĐỔI] Không còn ESegmentType.
    public string SegmentType { get; set; }

#pragma warning disable CS8618
    public OperationsSegmentViewModel()
#pragma warning restore CS8618
    {
    }

    public OperationsSegmentViewModel(
        string segmentId,
        string? description,
        string segmentType)
    {
        SegmentId = segmentId;
        Description = description;
        SegmentType = segmentType;
    }
}
