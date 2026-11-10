
namespace MesMicroservice.Application.Queries.EquipmentCapabilities;

public class EquipmentCapabilityPropertyUnitsQueryHandler : IRequestHandler<EquipmentCapabilityPropertyUnitsQuery, IEnumerable<string>>
{
    private readonly IEquipmentCapabilityRepository _equipmentCapabilityRepository;
    public EquipmentCapabilityPropertyUnitsQueryHandler(IEquipmentCapabilityRepository equipmentCapabilityRepository)
    {
        _equipmentCapabilityRepository = equipmentCapabilityRepository;
    }
    public async Task<IEnumerable<string>> Handle(EquipmentCapabilityPropertyUnitsQuery request, CancellationToken cancellationToken)
    {
        var equipmentCapabilities = await _equipmentCapabilityRepository.GetEquipmentCapabilities();

        var units = equipmentCapabilities
            .SelectMany(ec => ec.Properties)
            .Select(p => p.ValueUnitOfMeasure)
            .Distinct()
            .ToList();

        return units;
    }
}
