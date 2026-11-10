namespace MesMicroservice.Domain.AggregateModels.WorkOrderAggregate
{
    public class MaterialWorkOrder : IAggregateRoot
    {
        public string Id { get; private set; }
        public MaterialDefinition MaterialDefinition { get; private set; }
        public string ClassName { get; private set; }
        public Value Quantity { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }

        public int WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; private set; }
        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private MaterialWorkOrder() { }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MaterialWorkOrder(MaterialDefinition materialDefinition, string className, Value quantity, string valueUnitOfMeasure, WorkOrder workOrder)
        {
            Id = Guid.NewGuid().ToString();
            MaterialDefinition = materialDefinition;
            ClassName = className;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            WorkOrder = workOrder;
        }
    }
}
