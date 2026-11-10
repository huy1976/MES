

namespace MesMicroservice.Application.Queries.EquipmentClasses;

public class EquipmentClassIdsQueryHandler : IRequestHandler<EquipmentClassIdsQuery, IEnumerable<EquipmentClassIdViewModel>>
{
    private readonly IEquipmentClassRepository _equipmentClassRepository;

    public EquipmentClassIdsQueryHandler(IEquipmentClassRepository equipmentClassRepository)
    {
        _equipmentClassRepository = equipmentClassRepository;
    }

    public async Task<IEnumerable<EquipmentClassIdViewModel>> Handle(EquipmentClassIdsQuery request, CancellationToken cancellationToken)
    {
        var equipmentClasses = await _equipmentClassRepository.GetEquipmentClasses();

        return equipmentClasses.Select(x => new EquipmentClassIdViewModel(x.ResourceId, x.Name, x.Description));
    }
}
