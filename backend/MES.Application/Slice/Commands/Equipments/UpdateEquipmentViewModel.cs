namespace MesMicroservice.Application.Commands.Equipments;

[DataContract]
public class UpdateEquipmentViewModel
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public List<SavePropertyViewModel> Properties { get; set; }
    [DataMember]
    public string EquipmentClass { get; set; }
    [DataMember]
    public string? AbsolutePath { get; set; }

    public UpdateEquipmentViewModel(string name, List<SavePropertyViewModel> properties, string equipmentClass, string? absolutePath)
    {
        Name = name;
        Properties = properties;
        EquipmentClass = equipmentClass;
        AbsolutePath = absolutePath;
    }
}
