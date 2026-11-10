namespace MesMicroservice.Application.Queries.OperationsDefinitions;

public class OperationsSegmentDependencyViewModel
{
    public string OperationsSegmentDependencyId { get; set; }
    public string FromSegmentId { get; set; }
    public string ToSegmentId { get; set; }
    public string Description { get; set; }
    public EDependencyType DependencyType { get; set; }
    public Value DependencyFactor { get;  set; }
    public string ValueUnitOfMeasure { get;  set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public OperationsSegmentDependencyViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public OperationsSegmentDependencyViewModel(string operationsSegmentDependencyId, string fromSegmentId, string toSegmentId, string description, EDependencyType dependencyType, Value dependencyFactor, string valueUnitOfMeasure)
    {
        OperationsSegmentDependencyId = operationsSegmentDependencyId;
        FromSegmentId = fromSegmentId;
        ToSegmentId = toSegmentId;
        Description = description;
        DependencyType = dependencyType;
        DependencyFactor = dependencyFactor;
        ValueUnitOfMeasure = valueUnitOfMeasure;
    }
}
