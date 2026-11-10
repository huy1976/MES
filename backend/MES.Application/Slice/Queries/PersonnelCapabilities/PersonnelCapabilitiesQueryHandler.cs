
namespace MesMicroservice.Application.Queries.PersonnelCapabilities;

public class PersonnelCapabilitiesQueryHandler : IRequestHandler<PersonnelCapabilitiesQuery, IEnumrable<PersonnelCapabilityViewModel>>
{
    private readonly IPersonnelCapabilityRepository _personnelCapabilityRepository;
    private readonly IMapper _mapper;

    public PersonnelCapabilitiesQueryHandler(IPersonnelCapabilityRepository personnelCapabilityRepository, IMapper mapper)
    {
        _personnelCapabilityRepository = personnelCapabilityRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<PersonnelCapabilityViewModel>> Handle(PersonnelCapabilitiesQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _personnelCapabilityRepository.GetPersonnelCapabilities();
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

        var personnelCapabilities = queryable.ToList();

        var queryResult = new IEnumrable<PersonnelCapability>(personnelCapabilities, totalItems);

        return _mapper.Map<IEnumrable<PersonnelCapability>, IEnumrable<PersonnelCapabilityViewModel>>(queryResult);
    }
}
