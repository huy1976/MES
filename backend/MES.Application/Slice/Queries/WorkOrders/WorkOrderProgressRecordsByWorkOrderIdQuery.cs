namespace MesMicroservice.Application.Queries.WorkOrders;

public class WorkOrderProgressRecordsByWorkOrderIdQuery : IRequest<IEnumerable<WorkOrderProgressRecordDTO>>
{
    public string WorkOrderId { get; set; }

    public WorkOrderProgressRecordsByWorkOrderIdQuery(string workOrderId)
    {
        WorkOrderId = workOrderId;
    }
}
