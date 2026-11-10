
namespace MesMicroservice.Application.Queries.Persons;

public class PersonsQueryHandler : IRequestHandler<PersonsQuery, IEnumrable<PersonViewModel>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;

    public PersonsQueryHandler(IPersonRepository personRepository, IMapper mapper)
    {
        _personRepository = personRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<PersonViewModel>> Handle(PersonsQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _personRepository.GetPeople();
        if (request.IdStartedWith is not null)
        {
            queryable = queryable.Where(x => x.ResourceId.StartsWith(request.IdStartedWith));
        }

        if (request.Paginated)
        {
            queryable = queryable
                .OrderBy(x => x.ResourceId)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        int totalItems = queryable.Count();

        var persons = queryable.ToList();

        var queryResult = new IEnumrable<Person>(persons, totalItems);

        return _mapper.Map<IEnumrable<Person>, IEnumrable<PersonViewModel>>(queryResult);
    }
}
