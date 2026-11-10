
namespace MesMicroservice.Domain.AggregateModels.SegmentAggregate
{
    public class MaterialSegmentSpecification : Resource, IAggregateRoot
    {
        public string? Description { get; set; }
        public EAssemblyType AssemblyType { get; set; }
        public EAssemblyRelationship AssemblyRelationship { get; set; }
        public EMaterialUse MaterialUse { get; set; }
        public Value? Quantity { get; private set; }
        public string? ValueUnitOfMeasure { get; private set; }
        public ProcessSegment ProcessSegment { get; set; }
        public MaterialDefinition MaterialDefinition { get; set; }
        public ICollection<MaterialSegmentSpecificationProperty> Properties { get; set; } = new List<MaterialSegmentSpecificationProperty>();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private MaterialSegmentSpecification() : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MaterialSegmentSpecification(string materialSegmentSpecificationId, string? description, EAssemblyType assemblyType, EAssemblyRelationship assemblyRelationship,
                                            EMaterialUse materialUse, Value? quantity, string? valueUnitOfMeasure, ProcessSegment processSegment,
                                            MaterialDefinition materialDefinition, ICollection<MaterialSegmentSpecificationProperty> properties) : base(materialSegmentSpecificationId)
        {
            Description = description;
            AssemblyType = assemblyType;
            AssemblyRelationship = assemblyRelationship;
            MaterialUse = materialUse;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            ProcessSegment = processSegment;
            MaterialDefinition = materialDefinition;
            Properties = properties;
        }

    }
}
