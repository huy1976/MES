namespace MesMicroservice.Application.Queries.WorkOrders;

public class WorkOrderErrorRecordsByWorkOrderIdQuery : IRequest<IEnumerable<WorkOrderErrorRecordDTO>>
{
    public string WorkOrderId { get; set; }

    public WorkOrderErrorRecordsByWorkOrderIdQuery(string workOrderId)
    {
        WorkOrderId = workOrderId;
    }
}
