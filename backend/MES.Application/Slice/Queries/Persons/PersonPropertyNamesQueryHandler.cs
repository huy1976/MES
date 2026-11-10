
namespace MesMicroservice.Application.Queries.Persons;

public class PersonPropertyNamesQueryHandler : IRequestHandler<PersonPropertyNamesQuery, IEnumerable<string>>
{
    private readonly IPersonRepository _personRepository;
    public PersonPropertyNamesQueryHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }
    public async Task<IEnumerable<string>> Handle(PersonPropertyNamesQuery request, CancellationToken cancellationToken)
    {
       var people = await _personRepository.GetPeople();

        var propertyNames = people
            .SelectMany(p => p.Properties)
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        return propertyNames;
    }
}
