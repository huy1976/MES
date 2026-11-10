namespace MesMicroservice.Application.Queries.SegmentModels;

public class MaterialSegmentSpecificationsByProcessSegmentIdQueryHandler : IRequestHandler<MaterialSegmentSpecificationsByProcessSegmentIdQuery, IEnumrable<MaterialSegmentSpecificationViewModel>>
{
    private readonly IMaterialSegmentSpecificationRepository _materialSegmentSpecificationRepository;
    private readonly IMapper _mapper;
    public MaterialSegmentSpecificationsByProcessSegmentIdQueryHandler(IMaterialSegmentSpecificationRepository materialSegmentSpecificationRepository, IMapper mapper)
    {
        _materialSegmentSpecificationRepository = materialSegmentSpecificationRepository;
        _mapper = mapper;
    }
    public async Task<IEnumrable<MaterialSegmentSpecificationViewModel>> Handle(MaterialSegmentSpecificationsByProcessSegmentIdQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _materialSegmentSpecificationRepository.GetMaterialSegmentSpecificationsByProcessSegmentIdAsync(request.ProcessSegmentId);

        int totalItems = queryable.Count();

        var materialSegmentSpecifications = queryable.ToList();

        var queryResult = new IEnumrable<MaterialSegmentSpecification>(materialSegmentSpecifications, totalItems);

        return _mapper.Map<IEnumrable<MaterialSegmentSpecification>, IEnumrable<MaterialSegmentSpecificationViewModel>>(queryResult);
    }
}
