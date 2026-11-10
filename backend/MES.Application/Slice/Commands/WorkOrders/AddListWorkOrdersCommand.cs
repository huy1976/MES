namespace MesMicroservice.Application.Commands.WorkOrders
{
    public class AddListWorkOrdersCommand : IRequest<bool>
    {
        public List<WorkOrderDTO> workOrderDTOs { get; set; }

        public AddListWorkOrdersCommand(List<WorkOrderDTO> workOrderDTOs)
        {
            this.workOrderDTOs = workOrderDTOs;
        }
    }
}
