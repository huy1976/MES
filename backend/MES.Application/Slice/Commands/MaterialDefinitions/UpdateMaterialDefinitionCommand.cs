namespace MesMicroservice.Application.Commands.MaterialDefinitions;

public class UpdateMaterialDefinitionCommand: IRequest<bool>
{
    public string MaterialDefinitionId { get; set; }
    public string Name { get; set; }
    public string PrimaryUnit { get; set; }
    public List<SavePropertyViewModel> Properties { get; set; }
    public string MaterialClass { get; set; }

    public UpdateMaterialDefinitionCommand(string materialDefinitionId, string name, string primaryUnit, List<SavePropertyViewModel> properties, string materialClass)
    {
        MaterialDefinitionId = materialDefinitionId;
        Name = name;
        PrimaryUnit = primaryUnit;
        Properties = properties;
        MaterialClass = materialClass;
    }
}
