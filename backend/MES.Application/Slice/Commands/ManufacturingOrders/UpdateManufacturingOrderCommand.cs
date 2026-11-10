namespace MesMicroservice.Application.Commands.ManufacturingOrders;

public class UpdateManufacturingOrderCommand : IRequest<bool>
{
    public string ManufacturingOrderId { get; set; }
    public string MaterialDefinitionId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime AvailableDate { get; set; }
    public int Priority { get; set; }

    public UpdateManufacturingOrderCommand(string manufacturingOrderId, string materialDefinitionId, decimal quantity, string unit, DateTime dueDate, DateTime availableDate, int priority)
    {
        ManufacturingOrderId = manufacturingOrderId;
        MaterialDefinitionId = materialDefinitionId;
        Quantity = quantity;
        Unit = unit;
        DueDate = dueDate;
        AvailableDate = availableDate;
        Priority = priority;
    }
}
