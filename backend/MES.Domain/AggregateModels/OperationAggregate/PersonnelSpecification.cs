namespace MesMicroservice.Domain.AggregateModels.OperationAggregate
{
    public class PersonnelSpecification : Entity, IAggregateRoot
    {
        public string PersonnelSegmentSpecificationId { get; private set; }
        public string PersonnelClassId { get; private set; }
        public string? Description { get; set; }
        public EPersonnelUse PersonnelUse { get; set; }
        public Value Quantity { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }

        public OperationsSegment OperationsSegment { get; set; }

        public ICollection<PersonnelSpecificationProperty> Properties { get; set; } = new List<PersonnelSpecificationProperty>();

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private PersonnelSpecification() { }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public PersonnelSpecification(string personnelSegmentSpecificationId, string personnelClassId, string? description, EPersonnelUse personnelUse, Value quantity, string valueUnitOfMeasure, OperationsSegment operationsSegment, ICollection<PersonnelSpecificationProperty> properties)
        {
            PersonnelSegmentSpecificationId = personnelSegmentSpecificationId;
            PersonnelClassId = personnelClassId;
            Description = description;
            PersonnelUse = personnelUse;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            OperationsSegment = operationsSegment;
            Properties = properties;
        }
    }
}
