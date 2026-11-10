namespace MesMicroservice.Domain.AggregateModels.OperationAggregate
{
    public class OperationsSegment : Entity, IAggregateRoot
    {
        public string SegmentId { get; private set; }
        public string ProductionParameterId { get; private set; }
        public string? Description { get; set; }
        public EOperationsType OperationsType { get; set; }

        // [ĐÃ ĐỔI]
        // Không còn giới hạn Drawing / Enameling / Both.
        public string SegmentType { get; set; }

        public string? HierarchyScope { get; set; }
        public Value Duration { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }
        public string? WorkDefinitionId { get; set; }
        public string OutputMaterialDefinitionId { get; set; } = string.Empty;
        public OperationsDefinition OperationsDefinition { get; set; }
        public ICollection<EquipmentSpecification> EquipmentSpecifications { get; private set; } = new List<EquipmentSpecification>();
        public ICollection<PersonnelSpecification> PersonnelSpecifications { get; private set; } = new List<PersonnelSpecification>();
        public ICollection<MaterialSpecification> MaterialSpecifications { get; private set; } = new List<MaterialSpecification>();

        public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();

#pragma warning disable CS8618
        private OperationsSegment() { }
#pragma warning restore CS8618

        public OperationsSegment(
            string segmentId,
            string productionParameterId,
            string? description,
            EOperationsType operationsType,
            string segmentType,
            string? hierarchyScope,
            Value duration,
            string valueUnitOfMeasure,
            string? workDefinitionId,
            OperationsDefinition operationsDefinition,
            ICollection<EquipmentSpecification> equipmentSpecifications,
            ICollection<PersonnelSpecification> personnelSpecifications,
            ICollection<MaterialSpecification> materialSpecifications,
            string materialId)
        {
            SegmentId = segmentId;
            ProductionParameterId = productionParameterId;
            Description = description;
            OperationsType = operationsType;

            if (string.IsNullOrWhiteSpace(segmentType))
                throw new ArgumentException("SegmentType cannot be empty.", nameof(segmentType));

            SegmentType = segmentType.Trim();
            HierarchyScope = hierarchyScope;
            Duration = duration;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            WorkDefinitionId = workDefinitionId;
            OperationsDefinition = operationsDefinition;
            EquipmentSpecifications = equipmentSpecifications;
            PersonnelSpecifications = personnelSpecifications;
            MaterialSpecifications = materialSpecifications;
            OutputMaterialDefinitionId = materialId;
        }
    }
}
