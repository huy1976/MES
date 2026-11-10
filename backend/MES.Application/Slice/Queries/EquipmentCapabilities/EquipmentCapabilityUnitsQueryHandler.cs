
namespace MesMicroservice.Application.Queries.EquipmentCapabilities;

public class EquipmentCapabilityUnitsQueryHandler : IRequestHandler<EquipmentCapabilityUnitsQuery, IEnumerable<string>>
{
    private readonly IEquipmentCapabilityRepository _equipmentCapabilityRepository;
    public EquipmentCapabilityUnitsQueryHandler(IEquipmentCapabilityRepository equipmentCapabilityRepository)
    {
        _equipmentCapabilityRepository = equipmentCapabilityRepository;
    }
    public async Task<IEnumerable<string>> Handle(EquipmentCapabilityUnitsQuery request, CancellationToken cancellationToken)
    {
        var equipmentCapabilities = await _equipmentCapabilityRepository.GetEquipmentCapabilities();

        var units = equipmentCapabilities
            .Select(p => p.ValueUnitOfMeasure)
            .Distinct()
            .ToList();

        return units;

    }
}
