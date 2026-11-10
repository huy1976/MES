
namespace MesMicroservice.Application.Queries.SegmentModels;

public class PersonnelSegmentSpecificationsQueryHandler : IRequestHandler<PersonnelSegmentSpecificationsQuery, IEnumrable<PersonnelSegmentSpecificationViewModel>>
{
    private readonly IPersonnelSegmentSpecificationRepository _personnelSegmentSpecificationRepository;
    private readonly IMapper _mapper;

    public PersonnelSegmentSpecificationsQueryHandler(IPersonnelSegmentSpecificationRepository personnelSegmentSpecificationRepository, IMapper mapper)
    {
        _personnelSegmentSpecificationRepository = personnelSegmentSpecificationRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<PersonnelSegmentSpecificationViewModel>> Handle(PersonnelSegmentSpecificationsQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _personnelSegmentSpecificationRepository.GetPersonnelSegmentSpecifications();

        if (request.IdStartedWith is not null)
        {
            queryable = queryable.Where(x => x.ResourceId.StartsWith(request.IdStartedWith));
        }

        if (request.Paginated)
        {
            queryable = queryable
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        int totalItems = queryable.Count();

        var personnelSegmentSpecifications = queryable.ToList();

        var queryResult = new IEnumrable<PersonnelSegmentSpecification>(personnelSegmentSpecifications, totalItems);

        return _mapper.Map<IEnumrable<PersonnelSegmentSpecification>, IEnumrable<PersonnelSegmentSpecificationViewModel>>(queryResult);
    }
}
