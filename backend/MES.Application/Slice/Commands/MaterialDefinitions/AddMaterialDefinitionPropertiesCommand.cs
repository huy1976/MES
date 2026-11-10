namespace MesMicroservice.Application.Commands.MaterialDefinitions
{
    [DataContract]
    public class AddMaterialDefinitionPropertiesCommand : IRequest<bool>
    {
        [DataMember]
        public string MaterialDefinitionId { get; set; }
        [DataMember]
        public List<SavePropertyViewModel> Properties { get; set; }

        public AddMaterialDefinitionPropertiesCommand(string materialDefinitionId, List<SavePropertyViewModel> properties)
        {
            MaterialDefinitionId = materialDefinitionId;
            Properties = properties;
        }
    }
}
