
namespace MesMicroservice.Application.Queries.PersonnelCapabilities;

public class PersonnelCapabilityPropertyNamesQueryHandler : IRequestHandler<PersonnelCapabilityPropertyNamesQuery, IEnumerable<string>>
{
    private readonly IPersonnelCapabilityRepository _personnelCapabilityRepository;

    public PersonnelCapabilityPropertyNamesQueryHandler(IPersonnelCapabilityRepository personnelCapabilityRepository)
    {
        _personnelCapabilityRepository = personnelCapabilityRepository;
    }

    public async Task<IEnumerable<string>> Handle(PersonnelCapabilityPropertyNamesQuery request, CancellationToken cancellationToken)
    {
        var personnelCapabilities = await _personnelCapabilityRepository.GetPersonnelCapabilities();

        var propertyNames = personnelCapabilities
            .SelectMany(pc => pc.Properties)
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        return propertyNames;
    }
}
