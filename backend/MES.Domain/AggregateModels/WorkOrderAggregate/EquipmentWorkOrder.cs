namespace MesMicroservice.Domain.AggregateModels.WorkOrderAggregate
{
    public class EquipmentWorkOrder : IAggregateRoot
    {
        public string Id { get; private set; }
        public Equipment Equipment { get; private set; }
        public string ClassName { get; private set; }
        public int WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private EquipmentWorkOrder() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public EquipmentWorkOrder(Equipment equipment, string className, WorkOrder workOrder)
        {
            Id = Guid.NewGuid().ToString();
            Equipment = equipment;
            ClassName = className;
            WorkOrder = workOrder;
        }
    }
}
