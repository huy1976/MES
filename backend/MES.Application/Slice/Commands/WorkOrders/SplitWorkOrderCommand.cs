namespace MesMicroservice.Application.Commands.WorkOrders
{
    public class SplitWorkOrderCommand : IRequest<bool>
    {
        public int WorkOrderId { get; set; }
        public int SplitQuantity { get; set; }

        public SplitWorkOrderCommand(int workOrderId, int splitQuantity)
        {
            WorkOrderId = workOrderId;
            SplitQuantity = splitQuantity;
        }
    }
}
