namespace MesMicroservice.Application.Queries.SegmentModels;

public class MaterialSegmentSpecificationsQuery : PaginatedQuery, IRequest<IEnumrable<MaterialSegmentSpecificationViewModel>>
{
    public string? IdStartedWith { get; set; }
}
