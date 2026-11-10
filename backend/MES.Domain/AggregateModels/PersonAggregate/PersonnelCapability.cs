namespace MesMicroservice.Domain.AggregateModels.PersonAggregate
{
    public class PersonnelCapability : Resource, IAggregateRoot
    {
        public string Description { get; set; }
        public string HierarchyScope { get; set; }
        public ECapabilityType CapabilityType { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public Value Quantity { get; set; }
        public string ValueUnitOfMeasure { get; set; }
        public ICollection<PersonnelCapabilityProperty> Properties { get; set; } = new List<PersonnelCapabilityProperty>();

        public int PersonId { get; set; }
        public Person? Person { get; set; }

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private PersonnelCapability() { }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public PersonnelCapability(string personnelCapabilityId, string description, string hierarchyScope, ECapabilityType capabilityType, 
                                   DateTime validFrom, DateTime validTo, Value quantity, string valueUnitOfMeasure, 
                                   ICollection<PersonnelCapabilityProperty> properties, Person person) : base(personnelCapabilityId)
        {
            Description = description;
            HierarchyScope = hierarchyScope;
            CapabilityType = capabilityType;
            ValidFrom = validFrom;
            ValidTo = validTo;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            Properties = properties;
            Person = person;
        }

    }
}
