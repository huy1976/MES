
namespace MesMicroservice.Application.Queries.Equipments;

public class EquipmentPropertyNamesQueryHandler : IRequestHandler<EquipmentPropertyNamesQuery, IEnumerable<string>>
{
    private readonly IEquipmentRepository _equipmentRepository;

    public EquipmentPropertyNamesQueryHandler(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<IEnumerable<string>> Handle(EquipmentPropertyNamesQuery request, CancellationToken cancellationToken)
    {
        var equipments = await _equipmentRepository.GetEquipments();

        var propertyNames = equipments
            .SelectMany(e => e.Properties)
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        return propertyNames;

    }
}
