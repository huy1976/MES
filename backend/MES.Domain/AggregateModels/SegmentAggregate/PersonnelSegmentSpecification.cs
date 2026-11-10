namespace MesMicroservice.Domain.AggregateModels.SegmentAggregate
{
    public class PersonnelSegmentSpecification : Resource, IAggregateRoot
    {
        public string? Description { get; set; }
        public EPersonnelUse PersonnelUse { get; set; }
        public Value Quantity { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }

        public ProcessSegment ProcessSegment { get; set; }
        public PersonnelClass PersonnelClass { get; set; }
        public ICollection<PersonnelSegmentSpecificationProperty> Properties { get; set; } = new List<PersonnelSegmentSpecificationProperty>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private PersonnelSegmentSpecification() : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public PersonnelSegmentSpecification(string personnelSegmentSpecificationId, string? description, EPersonnelUse personnelUse, Value quantity, string valueUnitOfMeasure,
                                             ProcessSegment processSegment, PersonnelClass personnelClass,
                                             ICollection<PersonnelSegmentSpecificationProperty> properties) : base(personnelSegmentSpecificationId)
        {
            Description = description;
            PersonnelUse = personnelUse;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            ProcessSegment = processSegment;
            PersonnelClass = personnelClass;
            Properties = properties;
        }

    }
}
