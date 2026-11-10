namespace MesMicroservice.Application.Commands.MaterialClasses;

[DataContract]
public class AddMaterialClassPropertiesCommand : IRequest<bool>
{
    [DataMember]
    public string MaterialClassId { get; set; }

    [DataMember]
    public List<SavePropertyViewModel> Properties { get; set; }

    public AddMaterialClassPropertiesCommand(string materialClassId, List<SavePropertyViewModel> properties)
    {
        MaterialClassId = materialClassId;
        Properties = properties;
    }


}
