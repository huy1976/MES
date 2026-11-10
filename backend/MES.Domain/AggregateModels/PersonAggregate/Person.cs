

namespace MesMicroservice.Domain.AggregateModels.PersonAggregate
{
    public class Person : Resource, IAggregateRoot
    {

        public string Name { get; set; }
        public string? Description { get; set; }
        public EStatus PersonStatus { get; set; }
        public PersonnelClass PersonnelClass { get; private set; }
        public ICollection<PersonProperty> Properties { get; set; } = new List<PersonProperty>();


        public User? User { get; set; }
        public ICollection<PersonnelCapability> PersonnelCapabilities { get; set; } = new List<PersonnelCapability>();
        public ICollection<ManufacturingOrder> ManufacturingOrders { get; set; } = new List<ManufacturingOrder>();
        public ICollection<PersonWorkOrder> PersonWorkOrders { get; set; } = new List<PersonWorkOrder>();   

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Person() : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Person(string personId, string name, string? description, PersonnelClass personnelClass, ICollection<PersonProperty> properties) : base(personId)
        {
            Name = name;
            Description = description;
            PersonnelClass = personnelClass;
            Properties = properties;
            PersonStatus = EStatus.Available;
        }

        public void UpdateStatus(EStatus status)
        {
            PersonStatus = status;
        }

    }
}
