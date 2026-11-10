namespace MesMicroservice.Application.Commands.Equipments
{
    [DataContract]
    public class CreateEquipmentCapabilityCommand : IRequest<bool>
    {
        [DataMember]
        public string EquipmentCapabilityId { get; set; }
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
        public string EquipmentId { get; set; }

        public CreateEquipmentCapabilityCommand(string equipmentCapabilityId, ECapabilityType capabilityType, string? description, string? hierarchyScope, DateTime? validFrom, DateTime? validTo, Value quantity, string valueUnitOfMeasure, List<SavePropertyViewModel> properties, string equipmentId)
        {
            EquipmentCapabilityId = equipmentCapabilityId;
            CapabilityType = capabilityType;
            Description = description;
            HierarchyScope = hierarchyScope;
            ValidFrom = validFrom;
            ValidTo = validTo;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            Properties = properties;
            EquipmentId = equipmentId;
        }
    }
}