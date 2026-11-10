namespace MesMicroservice.Application.Commands.Equipments;

public class AddEquipmentPropertiesCommandHandler : IRequestHandler<AddEquipmentPropertiesCommand, bool>
{
    private readonly IEquipmentRepository _equipmentRepository;

    public AddEquipmentPropertiesCommandHandler(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<bool> Handle (AddEquipmentPropertiesCommand request, CancellationToken cancellationToken)
    {
        var equipment = await _equipmentRepository.GetAsync(request.EquipmentId);

        var properties = request.Properties.Select(x => new EquipmentProperty(
            x.PropertyId,
            x.Description,
            new Value(x.ValueString, x.ValueType),
            x.ValueUnitOfMeasure,
            x.Name))
            .ToList();

        if (equipment is null)
        {
            throw new ResourceNotFoundException(nameof(Equipment), request.EquipmentId);
        }

        equipment.AddProperties(properties);

        return await _equipmentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }


}
