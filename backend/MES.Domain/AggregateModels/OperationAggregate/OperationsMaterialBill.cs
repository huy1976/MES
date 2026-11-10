namespace MesMicroservice.Domain.AggregateModels.OperationAggregate
{
    public class OperationsMaterialBill : Entity, IAggregateRoot
    {
        public string? Description { get; set; }
        public ICollection<OperationsMaterialBillItem> Items { get; set; } = new List<OperationsMaterialBillItem>();
        public int OperationsDefinitionId { get; set; }
        public OperationsDefinition OperationsDefinition { get; set; }

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private OperationsMaterialBill() { }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public OperationsMaterialBill(string? description, OperationsDefinition operationsDefinition, ICollection<OperationsMaterialBillItem> items)
        {
            Description = description;
            OperationsDefinition = operationsDefinition;
            Items = items;
        }

    }
}
