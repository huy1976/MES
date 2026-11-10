
namespace MesMicroservice.Application.Queries.PersonnelClasses;

public class PersonnelClassPropertyNamesQueryHandler : IRequestHandler<PersonnelClassPropertyNamesQuery, IEnumerable<string>>
{
    private readonly IPersonnelClassRepository _personnelClassRepository;

    public PersonnelClassPropertyNamesQueryHandler(IPersonnelClassRepository personnelClassRepository)
    {
        _personnelClassRepository = personnelClassRepository;
    }

    public async Task<IEnumerable<string>> Handle(PersonnelClassPropertyNamesQuery request, CancellationToken cancellationToken)
    {
      var personnelClasses = await _personnelClassRepository.GetPersonnelClasses();

        var propertyNames = personnelClasses
            .SelectMany(pc => pc.Properties)
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        return propertyNames;
    }
}
