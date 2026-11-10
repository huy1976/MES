namespace MesMicroservice.Application.Commands.Equipments;

public class UpdateEquipmentCommand: IRequest<bool>
{
    public string EquipmentId { get; set; }
    public string Name { get; set; }
    public List<SavePropertyViewModel> Properties { get; set; }
    public string EquipmentClass { get; set; }
    public string? AbsolutePath { get; set; }

    public UpdateEquipmentCommand(string equipmentId, string name, List<SavePropertyViewModel> properties, string equipmentClass, string? absolutePath)
    {
        EquipmentId = equipmentId;
        Name = name;
        Properties = properties;
        EquipmentClass = equipmentClass;
        AbsolutePath = absolutePath;
    }
}
