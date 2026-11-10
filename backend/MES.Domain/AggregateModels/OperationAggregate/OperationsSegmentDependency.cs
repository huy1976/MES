namespace MesMicroservice.Domain.AggregateModels.OperationAggregate
{
    public class OperationsSegmentDependency : Entity, IAggregateRoot
    {
        public string FromSegmentId { get; set; }
        public string ToSegmentId { get; set; }
        public string Description { get; set; }
        public EDependencyType DependencyType { get; set; }
        public Value DependencyFactor { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }
        public bool IsActive { get; set; }
        public OperationsDefinition OperationsDefinition { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private OperationsSegmentDependency() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public OperationsSegmentDependency(string fromSegmentId, string toSegmentId, string description, EDependencyType dependencyType, Value dependencyFactor,
                                        string valueUnitOfMeasure, bool isActive, OperationsDefinition operationsDefinition)
        {
            FromSegmentId = fromSegmentId;
            ToSegmentId = toSegmentId;
            Description = description;
            DependencyType = dependencyType;
            DependencyFactor = dependencyFactor;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            IsActive = isActive;
            OperationsDefinition = operationsDefinition;
        }
    }
}
