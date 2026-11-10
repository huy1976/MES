namespace MesMicroservice.Domain.AggregateModels.OperationAggregate
{
    public class EquipmentSpecification : Entity, IAggregateRoot
    {
        public string EquipmentSegmentSpecificationId { get; private set; }
        public string EquipmentClassId { get; private set; }
        public string? Description { get; set; }
        public EEquipmentUse EquipmentUse { get; set; }
        public Value Quantity { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }

        public OperationsSegment OperationsSegment { get; set; }
        public ICollection<EquipmentSpecificationProperty> Properties { get; set; } = new List<EquipmentSpecificationProperty>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private EquipmentSpecification() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public EquipmentSpecification(string equipmentSegmentSpecificationId, string equipmentClassId, string? description, EEquipmentUse equipmentUse, Value quantity, string valueUnitOfMeasure, OperationsSegment operationsSegment, ICollection<EquipmentSpecificationProperty> properties)
        {
            EquipmentSegmentSpecificationId = equipmentSegmentSpecificationId;
            EquipmentClassId = equipmentClassId;
            Description = description;
            EquipmentUse = equipmentUse;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            OperationsSegment = operationsSegment;
            Properties = properties;
        }

    }
}
