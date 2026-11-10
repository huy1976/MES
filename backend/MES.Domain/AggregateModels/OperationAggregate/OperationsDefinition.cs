namespace MesMicroservice.Domain.AggregateModels.OperationAggregate
{
    public class OperationsDefinition : Resource, IAggregateRoot
    {
        public string Version { get; set; }
        public string? Description { get; set; }
        public EOperationsType OperationsType { get; set; }
        public string? HierarchyScope { get; set; }
        public string? WorkDefinitionId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public MaterialDefinition MaterialDefinition { get; set; }
        public ICollection<OperationsSegment> OperationsSegments { get; set; } = new List<OperationsSegment>();
        public ICollection<OperationsSegmentDependency> OperationsSegmentDependencies { get; set; } = new List<OperationsSegmentDependency>();
        public ICollection<ManufacturingOrder> ManufacturingOrders { get; set; } = new List<ManufacturingOrder>();
        public OperationsMaterialBill OperationsMaterialBill { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private OperationsDefinition() : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public OperationsDefinition(string operationId, string version, string? description, EOperationsType operationsType, string? hierarchyScope, 
                                    string? workDefinitionId, DateTime validFrom, DateTime? validTo, MaterialDefinition materialDefinition, 
                                    ICollection<OperationsSegment> operationsSegments, OperationsMaterialBill operationsMaterialBill) : base(operationId)
        {
            Version = version;
            Description = description;
            OperationsType = operationsType;
            HierarchyScope = hierarchyScope;
            WorkDefinitionId = workDefinitionId;
            ValidFrom = validFrom;
            ValidTo = validTo;
            MaterialDefinition = materialDefinition;
            OperationsSegments = operationsSegments;
            OperationsMaterialBill = operationsMaterialBill;
        }


    }
}
