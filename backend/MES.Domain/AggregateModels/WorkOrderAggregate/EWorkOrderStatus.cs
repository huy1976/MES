namespace MesMicroservice.Domain.AggregateModels.WorkOrderAggregate;
public enum EWorkOrderStatus
{
    Pending,
    Planned,
    ReadyForProduction,
    InProduction,
    Completed,
    Cancelled,
}
