namespace MesMicroservice.Application.Queries.ManufacturingOrders;

public class ManufacturingOrderQuery: IRequest<ManufacturingOrderDTO?>
{
    public string ManufacturingOrderId { get; set; }

    public ManufacturingOrderQuery(string manufacturingOrderId)
    {
        ManufacturingOrderId = manufacturingOrderId;
    }
}
