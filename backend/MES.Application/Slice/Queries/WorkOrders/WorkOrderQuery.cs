namespace MesMicroservice.Application.Queries.WorkOrders;

public class WorkOrderQuery: IRequest<WorkOrderViewDTO?>
{
    public string WorkOrderId { get; set; }
    public WorkOrderQuery(string workOrderId)
    {
        WorkOrderId = workOrderId;
    }

}
