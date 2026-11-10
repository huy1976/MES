using MesMicroservice.Domain.AggregateModels.WorkOrderAggregate;
namespace MesMicroservice.Domain.Exceptions;
[Serializable]
public class WorkOrderNotScheduledException : Exception
{
    public readonly string WorkOrderId = "";

    public WorkOrderNotScheduledException() : base()
    {
    }

    public WorkOrderNotScheduledException(string? message) : base(message)
    {
    }

    public WorkOrderNotScheduledException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
    public WorkOrderNotScheduledException(WorkOrder workOrder) : base($"Work order {workOrder.WorkOrderId} is not scheduled")
    {
        WorkOrderId = workOrder.WorkOrderId;
    }
}
