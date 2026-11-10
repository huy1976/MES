namespace MesMicroservice.Application.Queries.PersonnelCapabilities;

public class PersonnelCapabilitiesQuery : PaginatedQuery, IRequest<IEnumrable<PersonnelCapabilityViewModel>>
{
    public string? IdStartedWith { get; set; }
}
