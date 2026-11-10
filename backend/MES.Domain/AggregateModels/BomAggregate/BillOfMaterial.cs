namespace MesMicroservice.Domain.AggregateModels.BomAggregate
{
    public class BillOfMaterial : Resource, IAggregateRoot
    {
        public string? Description { get; set; }
        public EMaterialUse MaterialUse { get; set; }
        public EAssemblyType AssemblyType { get; set; }
        public EAssemblyRelationship AssemblyRelationship { get; set; }
        public Value Quantity { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }

        public MaterialDefinition MaterialDefinition { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private BillOfMaterial() : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public BillOfMaterial(string billId, string? description, EMaterialUse materialUse, EAssemblyType assemblyType, EAssemblyRelationship assemblyRelationship, 
                              Value quantity, string valueUnitOfMeasure, MaterialDefinition materialDefinition) : base(billId)
        {
            Description = description;
            MaterialUse = materialUse;
            AssemblyType = assemblyType;
            AssemblyRelationship = assemblyRelationship;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            MaterialDefinition = materialDefinition;
        }
    }
}
