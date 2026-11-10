namespace MesMicroservice.Domain.AggregateModels.EquipmentAggregate
{
    public class EquipmentCapability : Resource, IAggregateRoot
    {
        public string Description { get; set; }
        public string HierarchyScope { get; set; }
        public ECapabilityType CapabilityType { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public Value Quantity { get; set; }
        public string ValueUnitOfMeasure { get; set; }
        public ICollection<EquipmentCapabilityProperty> Properties { get; set; } = new List<EquipmentCapabilityProperty>();

        public int EquipmentId { get; set; }
        public Equipment? Equipment { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private EquipmentCapability() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public EquipmentCapability(string equipmentCapabilityId, string description, string hierarchyScope, ECapabilityType capabilityType,
                                   DateTime validFrom, DateTime validTo, Value quantity, string valueUnitOfMeasure,
                                   ICollection<EquipmentCapabilityProperty> properties, Equipment equipment) : base(equipmentCapabilityId)
        {
            Description = description;
            HierarchyScope = hierarchyScope;
            CapabilityType = capabilityType;
            ValidFrom = validFrom;
            ValidTo = validTo;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            Properties = properties;
            Equipment = equipment;
        }

        public void AddProperties(List<EquipmentCapabilityProperty> caPs)
        {
            foreach (var property in caPs)
            {
                Properties.Add(property);
            }

        }


    }
}
