namespace MesMicroservice.Application.Commands.Persons
{
    [DataContract]
    public class CreatePersonnelCapabilityCommand : IRequest<bool>
    {
        [DataMember]
        public string PersonnelCapabilityId { get; set; }
        [DataMember]
        public ECapabilityType CapabilityType { get; set; }
        [DataMember]
        public string? Description { get; set; }
        [DataMember]
        public string? HierarchyScope { get; set; }
        [DataMember]
        public DateTime? ValidFrom { get; set; }
        [DataMember]
        public DateTime? ValidTo { get; set; }
        [DataMember]
        public Value Quantity { get; set; }
        [DataMember]
        public string ValueUnitOfMeasure { get; set; }
        [DataMember]
        public List<SavePropertyViewModel> Properties { get; set; }
        [DataMember]
        public string PersonId { get; set; }
        public CreatePersonnelCapabilityCommand(string personnelCapabilityId, ECapabilityType capabilityType, string? description, string? hierarchyScope, DateTime? validFrom, DateTime? validTo, Value quantity, string valueUnitOfMeasure, List<SavePropertyViewModel> properties, string personId)
        {
            PersonnelCapabilityId = personnelCapabilityId;
            CapabilityType = capabilityType;
            Description = description;
            HierarchyScope = hierarchyScope;
            ValidFrom = validFrom;
            ValidTo = validTo;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            Properties = properties;
            PersonId = personId;
        }
    }
}
