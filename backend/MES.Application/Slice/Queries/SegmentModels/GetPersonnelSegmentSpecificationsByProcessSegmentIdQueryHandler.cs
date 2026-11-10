
namespace MesMicroservice.Application.Queries.SegmentModels;

public class GetPersonnelSegmentSpecificationsByProcessSegmentIdQueryHandler : IRequestHandler<GetPersonnelSegmentSpecificationsByProcessSegmentIdQuery, IEnumrable<PersonnelSegmentSpecificationViewModel>>
{
    private readonly IPersonnelSegmentSpecificationRepository _personnelSegmentSpecificationRepository;
    private readonly IMapper _mapper;
    public GetPersonnelSegmentSpecificationsByProcessSegmentIdQueryHandler(IPersonnelSegmentSpecificationRepository personnelSegmentSpecificationRepository, IMapper mapper)
    {
        _personnelSegmentSpecificationRepository = personnelSegmentSpecificationRepository;
        _mapper = mapper;
    }
    public async Task<IEnumrable<PersonnelSegmentSpecificationViewModel>> Handle(GetPersonnelSegmentSpecificationsByProcessSegmentIdQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _personnelSegmentSpecificationRepository.GetPersonnelSegmentSpecificationsByProcessSegmentIdAsync(request.ProcessSegmentId);

        int totalItems = queryable.Count();

        var personnelSegmentSpecifications = queryable.ToList();

        var queryResult = new IEnumrable<PersonnelSegmentSpecification>(personnelSegmentSpecifications, totalItems);

        return _mapper.Map<IEnumrable<PersonnelSegmentSpecification>, IEnumrable<PersonnelSegmentSpecificationViewModel>>(queryResult);

    }
}
