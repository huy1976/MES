
namespace MesMicroservice.Application.Queries.SegmentModels;

public class MaterialSegmentSpecificationsQueryHandler : IRequestHandler<MaterialSegmentSpecificationsQuery, IEnumrable<MaterialSegmentSpecificationViewModel>>
{
    private readonly IMaterialSegmentSpecificationRepository _materialSegmentSpecificationRepository;
    private readonly IMapper _mapper;

    public MaterialSegmentSpecificationsQueryHandler(IMaterialSegmentSpecificationRepository materialSegmentSpecificationRepository, IMapper mapper)
    {
        _materialSegmentSpecificationRepository = materialSegmentSpecificationRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<MaterialSegmentSpecificationViewModel>> Handle(MaterialSegmentSpecificationsQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _materialSegmentSpecificationRepository.GetMaterialSegmentSpecifications();

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

        var materialSegmentSpecifications = queryable.ToList();

        var queryResult = new IEnumrable<MaterialSegmentSpecification>(materialSegmentSpecifications, totalItems);

        return _mapper.Map<IEnumrable<MaterialSegmentSpecification>, IEnumrable<MaterialSegmentSpecificationViewModel>>(queryResult);
    }
}
