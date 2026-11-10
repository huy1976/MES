

namespace MesMicroservice.Application.Queries.PersonnelClasses;

public class PersonnelClassesQueryHandler : IRequestHandler<PersonnelClassesQuery, IEnumrable<PersonnelClassViewModel>>
{
    private readonly IPersonnelClassRepository _personnelClassRepository;
    private readonly IMapper _mapper;

    public PersonnelClassesQueryHandler(IPersonnelClassRepository personnelClassRepository, IMapper mapper)
    {
        _personnelClassRepository = personnelClassRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<PersonnelClassViewModel>> Handle(PersonnelClassesQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _personnelClassRepository.GetPersonnelClasses();
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

        var personnelClasses = queryable.ToList();

        var queryResult = new IEnumrable<PersonnelClass>(personnelClasses, totalItems);

        return _mapper.Map<IEnumrable<PersonnelClass>, IEnumrable<PersonnelClassViewModel>>(queryResult);

    }
}
