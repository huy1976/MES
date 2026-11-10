namespace MesMicroservice.Domain.AggregateModels.WorkCenterOutputAggregate;
public class WorkCenterOutput : Entity, IAggregateRoot
{
    public int WorkCenterId { get; private set; }
    public int MaterialDefinitionId { get; private set; }
    public MaterialDefinition MaterialDefinition { get; private set; }
    public WorkCenter WorkCenter { get; private set; }
    public decimal Output { get; private set; }
    public MaterialUnit Unit { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private WorkCenterOutput() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public WorkCenterOutput(MaterialDefinition materialDefinition, WorkCenter workCenter, decimal output, MaterialUnit unit)
    {
        MaterialDefinition = materialDefinition;
        WorkCenter = workCenter;
        Output = output;
        Unit = unit;
    }

    public void Update(decimal output, MaterialUnit unit)
    {
        Output = output;
        Unit = unit;
    }
}
