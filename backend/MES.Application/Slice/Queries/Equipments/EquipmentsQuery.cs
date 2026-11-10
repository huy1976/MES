namespace MesMicroservice.Application.Queries.Equipments;

public class EquipmentsQuery : PaginatedQuery, IRequest<IEnumrable<EquipmentViewModel>>
{
    public string? IdStartedWith { get; set; }
}
