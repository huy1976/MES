namespace MesMicroservice.Application.Commands.EquipmentClasses;

[DataContract]
public class CreateEquipmentClassCommand : IRequest<bool>
{
    [DataMember]
    public string EquipmentClassId { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string? Description { get; set; }
    [DataMember]
    public List<SavePropertyViewModel> Properties { get; set; }

    public CreateEquipmentClassCommand(string equipmentClassId, string name, string? description, List<SavePropertyViewModel> properties)
    {
        EquipmentClassId = equipmentClassId;
        Name = name;
        Description = description;
        Properties = properties;
    }
}
