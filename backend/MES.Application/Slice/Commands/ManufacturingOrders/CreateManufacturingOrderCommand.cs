namespace MesMicroservice.Application.Commands.ManufacturingOrders;

[DataContract]
public class CreateManufacturingOrderCommand : IRequest<bool>
{
    [DataMember]
    public string ManufacturingOrderId { get; set; }
    [DataMember]
    public string MaterialDefinitionId { get; set; }
    [DataMember]
    public decimal Quantity { get; set; }
    [DataMember]
    public string Unit { get; set; }
    [DataMember]
    public DateTime DueDate { get; set; }
    [DataMember]
    public string CustomerId { get; set; }
    [DataMember]
    public string OperationsDefinitionId { get; set; }
    [DataMember]
    public string QualityStandardId { get; set; }
    [DataMember]
    public string CreatedById { get; set; }
    [DataMember]
    public int Priority { get; set; } = 1;
    public CreateManufacturingOrderCommand(
    string manufacturingOrderId,
    string materialDefinitionId,
    decimal quantity,
    string unit,
    DateTime dueDate,
    string customerId,
    string operationsDefinitionId,
    string qualityStandardId,
    string createdById,
    int priority = 1)
    {
        ManufacturingOrderId = manufacturingOrderId;
        MaterialDefinitionId = materialDefinitionId;
        Quantity = quantity;
        Unit = unit;
        DueDate = dueDate;
        CustomerId = customerId;
        OperationsDefinitionId = operationsDefinitionId;
        QualityStandardId = qualityStandardId;
        CreatedById = createdById;
        Priority = priority;
    }
}


