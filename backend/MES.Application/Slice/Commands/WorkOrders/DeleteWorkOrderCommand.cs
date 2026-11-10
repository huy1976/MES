namespace MesMicroservice.Application.Commands.WorkOrders;

public class DeleteWorkOrderCommand : IRequest<bool>
{
    public string ManufacturingOrderId { get; set; }
    public string WorkOrderId { get; set; }

    public DeleteWorkOrderCommand(string manufacturingOrderId, string workOrderId)
    {
        ManufacturingOrderId = manufacturingOrderId;
        WorkOrderId = workOrderId;
    }
}
