namespace MesMicroservice.Domain.AggregateModels.PersonnelClassAggregate
{
    public class PersonnelClass : Resource, IAggregateRoot
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<PersonnelClassProperty> Properties { get; set; } = new List<PersonnelClassProperty>();
        public ICollection<Person> Persons { get; private set; } = new List<Person>();
        public ICollection<PersonnelSegmentSpecification> PersonnelSegmentSpecifications { get; set; } = new List<PersonnelSegmentSpecification>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private PersonnelClass() : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public PersonnelClass(string classId, string name, string? description, ICollection<PersonnelClassProperty> properties) : base(classId)
        {
            Name = name;
            Description = description;
            Properties = properties;
        }

    }
}
