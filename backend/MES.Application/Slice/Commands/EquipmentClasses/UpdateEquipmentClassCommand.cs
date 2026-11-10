namespace MesMicroservice.Application.Commands.EquipmentClasses;

public class UpdateEquipmentClassCommand: IRequest<bool>
{
    public string EquipmentClassId { get; set; }
    public string Name { get; set; }

    public UpdateEquipmentClassCommand(string equipmentClassId, string name)
    {
        EquipmentClassId = equipmentClassId;
        Name = name;
    }
}
