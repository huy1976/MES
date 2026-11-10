namespace MesMicroservice.Application.Commands.WorkOrders
{
    public class UpdateWorkOrderStatusCommand : IRequest<bool>
    {
        public string ManufacturingOrderId { get; set; }
        public string WorkOrderId { get; set; }
        public EStatus WorkOrderStatus { get; set; }

        public UpdateWorkOrderStatusCommand(string manufacturingOrderId, string workOrderId, EStatus workOrderStatus)
        {
            ManufacturingOrderId = manufacturingOrderId;
            WorkOrderId = workOrderId;
            WorkOrderStatus = workOrderStatus;
        }
    }
}
