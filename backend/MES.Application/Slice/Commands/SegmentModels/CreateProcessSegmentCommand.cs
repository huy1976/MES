namespace MesMicroservice.Application.Commands.SegmentModels
{
    [DataContract]
    public class CreateProcessSegmentCommand : IRequest<bool>
    {
        [DataMember]
        public string ProcessSegmentId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string? Description { get; set; }

        // [ĐÃ ĐỔI]
        // API có thể nhận "Drawing", "Enameling", "Packaging",
        // "QualityCheck", "Annealing", ... mà không cần sửa enum.
        [DataMember]
        public string SegmentType { get; set; }

        [DataMember]
        public EOperationsType OperationsType { get; set; }

        [DataMember]
        public string? HierarchyScope { get; set; }

        [DataMember]
        public string? DurationValueString { get; set; }

        [DataMember]
        public EValueType? ValueType { get; set; }

        [DataMember]
        public string? ValueUnitOfMeasure { get; set; }

        [DataMember]
        public string ProductionParameterId { get; set; }

        [DataMember]
        public string OutputMaterialDefinitionId { get; set; }

        public CreateProcessSegmentCommand(
            string processSegmentId,
            string name,
            string? description,
            string segmentType,
            EOperationsType operationsType,
            string? hierarchyScope,
            string? durationValueString,
            EValueType? valueType,
            string? valueUnitOfMeasure,
            string productionParameterId,
            string outputMaterialDefinitionId)
        {
            ProcessSegmentId = processSegmentId;
            Name = name;
            Description = description;
            SegmentType = segmentType;
            OperationsType = operationsType;
            HierarchyScope = hierarchyScope;
            DurationValueString = durationValueString;
            ValueType = valueType;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            ProductionParameterId = productionParameterId;
            OutputMaterialDefinitionId = outputMaterialDefinitionId;
        }
    }
}
