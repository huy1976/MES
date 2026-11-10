namespace MesMicroservice.Application.Commands.OperationsDefinitions
{
    public class CreateOperationSegmentDependencyCommand : IRequest<bool>
    {
        public string OperationsDefinitionId { get; set; }
        public string FromSegmentId { get; set; }
        public string ToSegmentId { get; set; }
        public string? Description { get; set; }
        public EDependencyType? DependencyType { get; set; }
        public Value? DependencyFactor { get; private set; }
        public string? ValueUnitOfMeasure { get; private set; }

        public CreateOperationSegmentDependencyCommand(string operationsDefinitionId, string fromSegmentId, string toSegmentId, string? description, EDependencyType? dependencyType, Value? dependencyFactor, string? valueUnitOfMeasure)
        {
            OperationsDefinitionId = operationsDefinitionId;
            FromSegmentId = fromSegmentId;
            ToSegmentId = toSegmentId;
            Description = description;
            DependencyType = dependencyType;
            DependencyFactor = dependencyFactor;
            ValueUnitOfMeasure = valueUnitOfMeasure;
        }
    }



}
