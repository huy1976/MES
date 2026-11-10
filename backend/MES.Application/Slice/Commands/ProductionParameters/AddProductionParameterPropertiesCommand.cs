namespace MesMicroservice.Application.Commands.ProductionParameters
{
    [DataContract]
    public class AddProductionParameterPropertiesCommand : IRequest<bool>
    {
        [DataMember]
        public string ProductionParameterId { get; set; }
        [DataMember]
        public List<SavePropertyViewModel> Properties { get; set; }

        public AddProductionParameterPropertiesCommand(string productionParameterId, List<SavePropertyViewModel> properties)
        {
            ProductionParameterId = productionParameterId;
            Properties = properties;
        }
    }
}
