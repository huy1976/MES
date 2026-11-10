namespace MesMicroservice.Application.Queries.EquipmentCapabilities;

public class EquipmentCapabilitiesQuery : PaginatedQuery, IRequest<IEnumrable<EquipmentCapabilityViewModel>>
{
    public string? IdStartedWith { get; set; }
}
