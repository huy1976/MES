namespace MesMicroservice.Application.Commands.ProductionParameters
{
    [DataContract]
    public class CreateProductionParameterCommand : IRequest<bool>
    {
        [DataMember]
        public string ProductionParameterId { get; set; }
        [DataMember]
        public string? Description { get; set; }
        [DataMember]
        public DateTime ValidFrom { get; set; }
        [DataMember]
        public DateTime? ValidTo { get; set; }
        [DataMember]
        public List<SavePropertyViewModel> Properties { get; set; }

        public CreateProductionParameterCommand(string productionParameterId, string? description, DateTime validFrom, DateTime? validTo, List<SavePropertyViewModel> properties)
        {
            ProductionParameterId = productionParameterId;
            Description = description;
            ValidFrom = validFrom;
            ValidTo = validTo;
            Properties = properties;
        }
    }
}
