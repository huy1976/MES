namespace MesMicroservice.Application.Commands.ManufacturingOrders;

public class DeleteManufacturingOrderCommand : IRequest<bool>
{
    public string ManufacturingOrderId { get; set; }

    public DeleteManufacturingOrderCommand(string manufacturingOrderId)
    {
        ManufacturingOrderId = manufacturingOrderId;
    }
}
