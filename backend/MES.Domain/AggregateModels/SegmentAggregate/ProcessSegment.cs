global using MesMicroservice.Domain.AggregateModels.ParameterAggregate;

namespace MesMicroservice.Domain.AggregateModels.SegmentAggregate
{
    public class ProcessSegment : Resource, IAggregateRoot
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        // [ĐÃ ĐỔI]
        // Trước đây: ESegmentType -> chỉ nhận Drawing / Enameling / Both
        // Bây giờ: string -> loại công đoạn lấy từ dữ liệu.
        public string SegmentType { get; set; }

        public EOperationsType OperationsType { get; set; }
        public string? HierarchyScope { get; set; }
        public Value Duration { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }
        public string OutputMaterialDefinitionId { get; set; } = string.Empty;
        public ProductionParameter ProductionParameter { get; set; }
        public ICollection<EquipmentSegmentSpecification> EquipmentSegmentSpecifications { get; set; } = new List<EquipmentSegmentSpecification>();
        public ICollection<MaterialSegmentSpecification> MaterialSegmentSpecifications { get; set; } = new List<MaterialSegmentSpecification>();
        public ICollection<PersonnelSegmentSpecification> PersonnelSegmentSpecifications { get; set; } = new List<PersonnelSegmentSpecification>();

#pragma warning disable CS8618
        private ProcessSegment() : base() { }
#pragma warning restore CS8618

        public ProcessSegment(
            string processSegmentId,
            string name,
            string? description,
            string segmentType,
            EOperationsType operationsType,
            string? hierarchyScope,
            Value duration,
            string valueUnitOfMeasure,
            ProductionParameter productionParameter,
            string materialId) : base(processSegmentId)
        {
            Name = name;
            Description = description;

            if (string.IsNullOrWhiteSpace(segmentType))
                throw new ArgumentException("SegmentType cannot be empty.", nameof(segmentType));

            SegmentType = segmentType.Trim();
            OperationsType = operationsType;
            HierarchyScope = hierarchyScope;
            Duration = duration;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            ProductionParameter = productionParameter;
            OutputMaterialDefinitionId = materialId;
        }
    }
}
