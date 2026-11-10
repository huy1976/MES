namespace MesMicroservice.Domain.AggregateModels.MaterialLotAggregate
{
    public class MaterialSubLot : Resource, IAggregateRoot
    {
        public string Description { get; private set; }
        public EAssemblyType AssemblyType { get; private set; } = EAssemblyType.None;
        public EAssemblyRelationship AssemblyRelationship { get; private set; } = EAssemblyRelationship.None;
        public EStatus Status { get; private set; } = EStatus.Available;
        public string StorageLocation { get; private set; }
        public decimal Quantity { get; private set; }
        public MaterialUnit Unit { get; private set; }
        public MaterialDefinition MaterialDefinition { get; private set; }
        public MaterialLot ParentMaterialLot { get; private set; }

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private MaterialSubLot() : base() { }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MaterialSubLot(string materialSubLotId, string description, EAssemblyType assemblyType, EAssemblyRelationship assemblyRelationship, EStatus status, string storageLocation, decimal quantity, MaterialUnit unit, MaterialDefinition materialDefinition, MaterialLot parentMaterialLot) : base(materialSubLotId)
        {
            Description = description;
            AssemblyType = assemblyType;
            AssemblyRelationship = assemblyRelationship;
            Status = status;
            StorageLocation = storageLocation;
            Quantity = quantity;
            Unit = unit;
            MaterialDefinition = materialDefinition;
            ParentMaterialLot = parentMaterialLot;
        }
    }
}
