namespace MesMicroservice.Domain.AggregateModels.OperationAggregate
{
    public class MaterialSpecification : Entity, IAggregateRoot
    {
        public string MaterialSegmentSpecificationId { get; private set; }
        public string MaterialDefinitionId { get; private set; }
        public string? Description { get; set; }
        public EAssemblyType AssemblyType { get; set; }
        public EAssemblyRelationship AssemblyRelationship { get; set; }
        public EMaterialUse MaterialUse { get; set; }
        public Value Quantity { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }

        public OperationsSegment OperationsSegment { get; set; }
        public ICollection<MaterialSpecificationProperty> Properties { get; set; } = new List<MaterialSpecificationProperty>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private MaterialSpecification() : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MaterialSpecification(string materialSegmentSpecificationId, string materialDefinitionId, string? description, EAssemblyType assemblyType, EAssemblyRelationship assemblyRelationship,
                                            EMaterialUse materialUse, Value quantity, string valueUnitOfMeasure, OperationsSegment operationsSegment,
                                            ICollection<MaterialSpecificationProperty> properties)
        {
            MaterialSegmentSpecificationId = materialSegmentSpecificationId;
            MaterialDefinitionId = materialDefinitionId;
            Description = description;
            AssemblyType = assemblyType;
            AssemblyRelationship = assemblyRelationship;
            MaterialUse = materialUse;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            OperationsSegment = operationsSegment;
            Properties = properties;
        }
    }
}
