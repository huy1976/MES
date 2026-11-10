

namespace MesMicroservice.Domain.AggregateModels.SegmentAggregate
{
    public class EquipmentSegmentSpecification : Resource, IAggregateRoot
    {
        public string? Description { get; set; }
        public EEquipmentUse EquipmentUse { get; set; }
        public Value Quantity { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }

        public ProcessSegment ProcessSegment { get; set; }
        public EquipmentClass EquipmentClass { get; set; }
        public ICollection<EquipmentSegmentSpecificationProperty> Properties { get; set; } = new List<EquipmentSegmentSpecificationProperty>();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private EquipmentSegmentSpecification() : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public EquipmentSegmentSpecification(string equipmentSegmentSpecificationId, string? description, EEquipmentUse equipmentUse, Value quantity, string valueUnitOfMeasure, 
                                             ProcessSegment processSegment, EquipmentClass equipmentClass, ICollection<EquipmentSegmentSpecificationProperty> properties) : base(equipmentSegmentSpecificationId)
        {
            Description = description;
            EquipmentUse = equipmentUse;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            ProcessSegment = processSegment;
            EquipmentClass = equipmentClass;
            Properties = properties;
        }

    }
}
