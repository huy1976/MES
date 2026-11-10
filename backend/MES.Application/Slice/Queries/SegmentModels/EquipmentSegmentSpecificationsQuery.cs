namespace MesMicroservice.Application.Queries.SegmentModels;

public class EquipmentSegmentSpecificationsQuery : PaginatedQuery, IRequest<IEnumrable<EquipmentSegmentSpecificationViewModel>>
{
    public string? IdStartedWith { get; set; }
}

