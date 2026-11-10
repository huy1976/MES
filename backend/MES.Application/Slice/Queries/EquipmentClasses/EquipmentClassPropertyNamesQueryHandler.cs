
namespace MesMicroservice.Application.Queries.EquipmentClasses;

public class EquipmentClassPropertyNamesQueryHandler : IRequestHandler<EquipmentClassPropertyNamesQuery, IEnumerable<string>>
{
    private readonly IEquipmentClassRepository _equipmentClassRepository;

    public EquipmentClassPropertyNamesQueryHandler(IEquipmentClassRepository equipmentClassRepository)
    {
        _equipmentClassRepository = equipmentClassRepository;
    }

    public async Task<IEnumerable<string>> Handle(EquipmentClassPropertyNamesQuery request, CancellationToken cancellationToken)
    {
        var equipmentClasses = await _equipmentClassRepository.GetEquipmentClasses();

        var propertyNames = equipmentClasses
            .SelectMany(ec => ec.Properties)
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        return propertyNames;
    }
}
