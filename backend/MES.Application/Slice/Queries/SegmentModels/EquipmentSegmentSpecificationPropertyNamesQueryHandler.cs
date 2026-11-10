
namespace MesMicroservice.Application.Queries.SegmentModels;

public class EquipmentSegmentSpecificationPropertyNamesQueryHandler : IRequestHandler<EquipmentSegmentSpecificationPropertyNamesQuery, IEnumerable<string>>
{
    private readonly IEquipmentSegmentSpecificationRepository _equipmentSegmentSpecificationRepository;
    public EquipmentSegmentSpecificationPropertyNamesQueryHandler(IEquipmentSegmentSpecificationRepository equipmentSegmentSpecificationRepository)
    {
        _equipmentSegmentSpecificationRepository = equipmentSegmentSpecificationRepository;
    }
    public async Task<IEnumerable<string>> Handle(EquipmentSegmentSpecificationPropertyNamesQuery request, CancellationToken cancellationToken)
    {
        var equipmentSegmentSpecifications = await _equipmentSegmentSpecificationRepository.GetEquipmentSegmentSpecifications();

        var propertyNames = equipmentSegmentSpecifications
            .SelectMany(ess => ess.Properties)
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        return propertyNames;
    }
}
