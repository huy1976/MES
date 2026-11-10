namespace MesMicroservice.Application.Queries.SegmentModels;

public class PersonnelSegmentSpecificationsQuery : PaginatedQuery, IRequest<IEnumrable<PersonnelSegmentSpecificationViewModel>>
{
    public string? IdStartedWith { get; set; }
}
