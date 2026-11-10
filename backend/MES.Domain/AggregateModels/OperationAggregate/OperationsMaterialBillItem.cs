namespace MesMicroservice.Domain.AggregateModels.OperationAggregate
{
    public class OperationsMaterialBillItem : Entity, IAggregateRoot
    {
        public string BillId { get; private set; }
        public string MaterialDefinitionId { get; set; }
        public string? Description { get; set; }
        public EMaterialUse MaterialUse { get; set; }
        public EAssemblyType AssemblyType { get; set; }
        public EAssemblyRelationship AssemblyRelationship { get; set; }
        public Value Quantity { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }

        public OperationsMaterialBill OperationsMaterialBill { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private OperationsMaterialBillItem() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


        public OperationsMaterialBillItem(string billId, string materialDefinitionId, string? description, EMaterialUse materialUse, EAssemblyType assemblyType, EAssemblyRelationship assemblyRelationship, Value quantity, string valueUnitOfMeasure, OperationsMaterialBill operationsMaterialBill)
        {
            BillId = billId;
            MaterialDefinitionId = materialDefinitionId;
            Description = description;
            MaterialUse = materialUse;
            AssemblyType = assemblyType;
            AssemblyRelationship = assemblyRelationship;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            OperationsMaterialBill = operationsMaterialBill;
        }
    }
}
