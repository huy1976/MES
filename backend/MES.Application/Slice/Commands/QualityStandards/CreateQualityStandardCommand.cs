namespace MesMicroservice.Application.Commands.QualityStandards
{
    [DataContract]
    public class CreateQualityStandardCommand : IRequest<bool>
    {
        [DataMember]
        public string QualityStandardId { get; set; }
        [DataMember]
        public string? Description { get; set; }
        [DataMember]
        public DateTime ValidFrom { get; set; }
        [DataMember]
        public DateTime? ValidTo { get; set; }
        [DataMember]
        public List<SavePropertyViewModel> Properties { get; set; }
        [DataMember]
        public List<string> MaterialDefinitionIds { get; set; }

        public CreateQualityStandardCommand(string qualityStandardId, string? description, DateTime validFrom, DateTime? validTo, 
                                            List<SavePropertyViewModel> properties, List<string> materialDefinitionIds)
        {
            QualityStandardId = qualityStandardId;
            Description = description;
            ValidFrom = validFrom;
            ValidTo = validTo;
            Properties = properties;
            MaterialDefinitionIds = materialDefinitionIds;
        }
    }
}