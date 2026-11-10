
namespace MesMicroservice.Application.Queries.Persons;

public class PersonIdsQueryHandler : IRequestHandler<PersonIdsQuery, IEnumerable<string>>
{
    private readonly IPersonRepository _personRepository;
    public PersonIdsQueryHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }
    public async Task<IEnumerable<string>> Handle(PersonIdsQuery request, CancellationToken cancellationToken)
    {
        var persons = await _personRepository.GetPeople();

        return persons.Select(x => x.ResourceId);
    }
}
