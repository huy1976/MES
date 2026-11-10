
namespace MesMicroservice.Application.Queries.EquipmentCapabilities;

public class EquipmentCapabilityPropertyNamesQueryHandler : IRequestHandler<EquipmentCapabilityPropertyNamesQuery, IEnumerable<string>>
{
    private readonly IEquipmentCapabilityRepository _equipmentCapabilityRepository;

    public EquipmentCapabilityPropertyNamesQueryHandler(IEquipmentCapabilityRepository equipmentCapabilityRepository)
    {
        _equipmentCapabilityRepository = equipmentCapabilityRepository;
    }

    public async Task<IEnumerable<string>> Handle(EquipmentCapabilityPropertyNamesQuery request, CancellationToken cancellationToken)
    {
       var equipmentCapabilities = await _equipmentCapabilityRepository.GetEquipmentCapabilities();

        var propertyNames = equipmentCapabilities
            .SelectMany(ec => ec.Properties)
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        return propertyNames;
    }
}
