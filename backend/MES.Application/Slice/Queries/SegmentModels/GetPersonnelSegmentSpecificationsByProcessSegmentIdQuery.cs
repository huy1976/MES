namespace MesMicroservice.Application.Queries.SegmentModels;

public class GetPersonnelSegmentSpecificationsByProcessSegmentIdQuery : IRequest<IEnumrable<PersonnelSegmentSpecificationViewModel>>
{
    public string ProcessSegmentId { get; set; }
    public GetPersonnelSegmentSpecificationsByProcessSegmentIdQuery(string processSegmentId)
    {
        ProcessSegmentId = processSegmentId;
    }
}
