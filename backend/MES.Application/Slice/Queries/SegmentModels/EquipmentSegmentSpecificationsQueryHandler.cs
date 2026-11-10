
namespace MesMicroservice.Application.Queries.SegmentModels;

public class EquipmentSegmentSpecificationsQueryHandler : IRequestHandler<EquipmentSegmentSpecificationsQuery, IEnumrable<EquipmentSegmentSpecificationViewModel>>
{
    private readonly IEquipmentSegmentSpecificationRepository _equipmentSegmentSpecificationRepository;
    private readonly IMapper _mapper;

    public EquipmentSegmentSpecificationsQueryHandler(IEquipmentSegmentSpecificationRepository equipmentSegmentSpecificationRepository, IMapper mapper)
    {
        _equipmentSegmentSpecificationRepository = equipmentSegmentSpecificationRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<EquipmentSegmentSpecificationViewModel>> Handle(EquipmentSegmentSpecificationsQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _equipmentSegmentSpecificationRepository.GetEquipmentSegmentSpecifications();

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

        var equipmentlSegmentSpecifications = queryable.ToList();

        var queryResult = new IEnumrable<EquipmentSegmentSpecification>(equipmentlSegmentSpecifications, totalItems);

        return _mapper.Map<IEnumrable<EquipmentSegmentSpecification>, IEnumrable<EquipmentSegmentSpecificationViewModel>>(queryResult);
    }
}
