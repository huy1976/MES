namespace MesMicroservice.Application.Queries.SegmentModels;

public class MaterialSegmentSpecificationsByProcessSegmentIdQuery : IRequest<IEnumrable<MaterialSegmentSpecificationViewModel>>
{
    public string ProcessSegmentId { get; set; }
    public MaterialSegmentSpecificationsByProcessSegmentIdQuery(string processSegmentId)
    {
        ProcessSegmentId = processSegmentId;
    }
}
