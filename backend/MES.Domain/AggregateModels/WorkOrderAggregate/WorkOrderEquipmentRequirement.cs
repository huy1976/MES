using MesMicroservice.Domain.AggregateModels.EquipmentClassAggregate;

namespace MesMicroservice.Domain.AggregateModels.WorkOrderAggregate
{
    public class WorkOrderEquipmentRequirement
    {
        public int Id { get; private set; }
        public int WorkOrderId { get; private set; }
        public List<EquipmentClass> EquipmentClasses { get; private set; }
        public int Quantity { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private WorkOrderEquipmentRequirement() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public WorkOrderEquipmentRequirement(List<EquipmentClass> equipmentClasses, int quantity)
        {
            EquipmentClasses = equipmentClasses;
            Quantity = quantity;
        }
    }
}
