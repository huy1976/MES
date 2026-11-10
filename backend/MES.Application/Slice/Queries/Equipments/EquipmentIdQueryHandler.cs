namespace MesMicroservice.Application.Queries.Equipments;

public class EquipmentIdQueryHandler : IRequestHandler<EquipmentIdQuery, IEnumerable<EquipmentIdViewModel>>
{
    private readonly IEquipmentRepository _equipmentRepository;
    public EquipmentIdQueryHandler(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<IEnumerable<EquipmentIdViewModel>> Handle(EquipmentIdQuery request, CancellationToken cancellationToken)
    {
        var equipments = await _equipmentRepository.GetAllAsync();

        return equipments.Select(x => new EquipmentIdViewModel(x.ResourceId, x.Name));
    }
}
