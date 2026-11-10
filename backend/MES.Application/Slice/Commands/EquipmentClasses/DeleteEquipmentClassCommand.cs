namespace MesMicroservice.Application.Commands.EquipmentClasses;

public class DeleteEquipmentClassCommand: IRequest<bool>
{
    public string EquipmentClassId { get; set; }

    public DeleteEquipmentClassCommand(string equipmentClassId)
    {
        EquipmentClassId = equipmentClassId;
    }
}
