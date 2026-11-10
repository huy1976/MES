namespace MesMicroservice.Application.Queries.EquipmentClasses;

public class EquipmentClassesQuery : PaginatedQuery, IRequest<IEnumrable<EquipmentClassViewModel>>
{
    public string? IdStartedWith { get; set; }
}
