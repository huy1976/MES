namespace MesMicroservice.Application.Commands.WorkOrders
{
    public class ConfirmListWorkOrdersCommand : IRequest<bool>
    {
        public List<ScheduledWorkOrderDTO> ScheduledWorkOrderDTOs { get; set; }

        public ConfirmListWorkOrdersCommand(List<ScheduledWorkOrderDTO> scheduledWorkOrderDTOs)
        {
            this.ScheduledWorkOrderDTOs = scheduledWorkOrderDTOs;
        }
    }
}
