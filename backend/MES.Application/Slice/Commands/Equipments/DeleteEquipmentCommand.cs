namespace MesMicroservice.Application.Commands.Equipments;

public class DeleteEquipmentCommand: IRequest<bool>
{
    public string EquipmentId { get; set; }

    public DeleteEquipmentCommand(string equipmentId)
    {
        EquipmentId = equipmentId;
    }
}
