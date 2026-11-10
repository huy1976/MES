global using MesMicroservice.Application.DTOs.WorkOrderDTOs;

namespace MesMicroservice.Application.Commands.WorkOrders
{
    public class CreateListWorkOrdersCommand : IRequest<List<WorkOrderDTO>>
    {
        public string ManufacturingOrderId { get; set; }

        public CreateListWorkOrdersCommand(string manufacturingOrderId)
        {
            ManufacturingOrderId = manufacturingOrderId;
        }
    }
}
