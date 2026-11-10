namespace MesMicroservice.Application.Commands.ManufacturingOrders;
[DataContract]
public class UpdateManufacturingOrderViewModel
{
    [DataMember]
    public string MaterialDefinitionId { get; set; }
    [DataMember]
    public decimal Quantity { get; set; }
    [DataMember]
    public string Unit { get; set; }
    [DataMember]
    public DateTime DueDate { get; set; }
    [DataMember]
    public DateTime AvailableDate { get; set; }
    [DataMember]
    public int Priority { get; set; }

    public UpdateManufacturingOrderViewModel(string materialDefinitionId, decimal quantity, string unit, DateTime dueDate, DateTime availableDate, int priority)
    {
        MaterialDefinitionId = materialDefinitionId;
        Quantity = quantity;
        Unit = unit;
        DueDate = dueDate;
        AvailableDate = availableDate;
        Priority = priority;
    }
}
