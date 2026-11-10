namespace MesMicroservice.Application.Commands.OperationsDefinitions
{
    [DataContract]
    public class CreateOperationsDefinitionCommand : IRequest<bool>
    {
        [DataMember]
        public string OperationsDefinitionId { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string MaterialId { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public EOperationsType? OperationsType { get; set; }
        [DataMember]
        public DateTime ValidFrom { get; set; }
        [DataMember]
        public DateTime? ValidTo { get; set; }
        [DataMember]
        public string? HierarchyScope { get; set; }
        [DataMember]
        public string? WorkDefinitionId { get; set; }
        [DataMember]
        public List<string> BomIds { get; set; }
        [DataMember]
        public List<string> SegmentIds { get; set; }

        [DataMember]
        public List<OperationDependencyInput> Dependencies { get; set; } = new();

        public CreateOperationsDefinitionCommand(string operationsDefinitionId, string description, string materialId, string version, EOperationsType? operationsType, DateTime validFrom, DateTime? validTo, string? hierarchyScope, string? workDefinitionId, List<string> bomIds, List<string> segmentIds)
        {
            OperationsDefinitionId = operationsDefinitionId;
            Description = description;
            MaterialId = materialId;
            Version = version;
            OperationsType = operationsType;
            ValidFrom = validFrom;
            ValidTo = validTo;
            HierarchyScope = hierarchyScope;
            WorkDefinitionId = workDefinitionId;
            BomIds = bomIds;
            SegmentIds = segmentIds;
        }
    }

    public class OperationDependencyInput
    {
        public string FromSegmentId { get; set; } = string.Empty;
        public string ToSegmentId { get; set; } = string.Empty;
    }


}
