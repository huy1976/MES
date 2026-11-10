namespace MesMicroservice.Application.Commands.EquipmentClasses;

public class CreateEquipmentClassCommandHandler : IRequestHandler<CreateEquipmentClassCommand, bool>
{
    private readonly IEquipmentClassRepository _equipmentClassRepository;

    public CreateEquipmentClassCommandHandler(IEquipmentClassRepository equipmentClassRepository)
    {
        _equipmentClassRepository = equipmentClassRepository;
    }

    public async Task<bool> Handle(CreateEquipmentClassCommand request, CancellationToken cancellationToken)
    {
        var properties = new List<EquipmentClassProperty>();
        if (request.Properties.Count() != 0)
        {
            properties = request.Properties.ConvertAll(x => new EquipmentClassProperty(
                x.PropertyId,
                x.Description,
                new Value(x.ValueString, x.ValueType),
                x.ValueUnitOfMeasure,
                x.Name));
        }

        var equipmentClass = new EquipmentClass(classId: request.EquipmentClassId,
                                                name: request.Name,
                                                description: request.Description ?? string.Empty,
                                                equipmentLevel: EEquipmentLevel.Equipment,
                                                properties: properties);

        await _equipmentClassRepository.AddAsync(equipmentClass);

        return await _equipmentClassRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
