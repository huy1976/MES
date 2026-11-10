namespace MesMicroservice.Application.Queries.SegmentModels;

public class EquipmentSegmentSpecificationsByProcessSegmentIdQueryHandler : IRequestHandler<EquipmentSegmentSpecificationsByProcessSegmentIdQuery, IEnumrable<EquipmentSegmentSpecificationViewModel>>
{
    private readonly IEquipmentSegmentSpecificationRepository _equipmentSegmentSpecificationRepository;
    private readonly IMapper _mapper;
    public EquipmentSegmentSpecificationsByProcessSegmentIdQueryHandler(IEquipmentSegmentSpecificationRepository equipmentSegmentSpecificationRepository, IMapper mapper)
    {
        _equipmentSegmentSpecificationRepository = equipmentSegmentSpecificationRepository;
        _mapper = mapper;
    }
    public async Task<IEnumrable<EquipmentSegmentSpecificationViewModel>> Handle(EquipmentSegmentSpecificationsByProcessSegmentIdQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _equipmentSegmentSpecificationRepository.GetEquipmentSegmentSpecificationsByProcessSegmentIdAsync(request.ProcessSegmentId);

        int totalItems = queryable.Count();

        var equipmentlSegmentSpecifications = queryable.ToList();

        var queryResult = new IEnumrable<EquipmentSegmentSpecification>(equipmentlSegmentSpecifications, totalItems);

        return _mapper.Map<IEnumrable<EquipmentSegmentSpecification>, IEnumrable<EquipmentSegmentSpecificationViewModel>>(queryResult);
    }
}
