namespace MesMicroservice.Application.Commands.MaterialDefinitions;

[DataContract]
public class UpdateMaterialDefinitionViewModel
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string PrimaryUnit { get; set; }
    [DataMember]
    public List<SavePropertyViewModel> Properties { get; set; }
    [DataMember]
    public string MaterialClass { get; set; }

    public UpdateMaterialDefinitionViewModel(string name, string primaryUnit, List<SavePropertyViewModel> properties, string materialClass)
    {
        Name = name;
        PrimaryUnit = primaryUnit;
        Properties = properties;
        MaterialClass = materialClass;
    }
}
