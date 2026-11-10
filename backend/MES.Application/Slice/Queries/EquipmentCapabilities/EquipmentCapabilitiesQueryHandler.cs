namespace MesMicroservice.Application.Queries.EquipmentCapabilities;

public class EquipmentCapabilitiesQueryHandler : IRequestHandler<EquipmentCapabilitiesQuery, IEnumrable<EquipmentCapabilityViewModel>>
{
    private readonly IEquipmentCapabilityRepository _equipmentCapabilityRepository;
    private readonly IMapper _mapper;

    public EquipmentCapabilitiesQueryHandler(IEquipmentCapabilityRepository equipmentCapabilityRepository, IMapper mapper)
    {
        _equipmentCapabilityRepository = equipmentCapabilityRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<EquipmentCapabilityViewModel>> Handle(EquipmentCapabilitiesQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _equipmentCapabilityRepository.GetEquipmentCapabilities();
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

        var equipmentCapabilities = queryable.ToList();

        var queryResult = new IEnumrable<EquipmentCapability>(equipmentCapabilities, totalItems);

        return _mapper.Map<IEnumrable<EquipmentCapability>, IEnumrable<EquipmentCapabilityViewModel>>(queryResult);
    }
}
