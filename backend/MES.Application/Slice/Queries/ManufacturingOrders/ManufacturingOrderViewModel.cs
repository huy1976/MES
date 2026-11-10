
namespace MesMicroservice.Application.Queries.ManufacturingOrders;

public class ManufacturingOrderViewModel
{
    public string ManufacturingOrderId { get; set; }
    public MaterialDefinitionViewModel MaterialDefinition { get; set; }
    public PersonViewModel CreatedBy { get; set; }
    public BusinessPartnerViewModel Customer { get; set; }
    public OperationsDefinitionViewModel OperationsDefinition { get; set; }
    public QualityStandardViewModel QualityStandard { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime AvailableDate { get; set; }
    public DateTime CreateDate { get; set; }
    public List<string> WorkOrders { get; set; }
    public int Priority { get; set; }
    public EStatus Status { get; set; }

    public ManufacturingOrderViewModel(string manufacturingOrderId, MaterialDefinitionViewModel materialDefinition, PersonViewModel createdBy, BusinessPartnerViewModel customer, OperationsDefinitionViewModel operationsDefinition, QualityStandardViewModel qualityStandard, decimal quantity, string unit, DateTime dueDate, DateTime availableDate, DateTime createDate, List<string> workOrders, int priority, EStatus status)
    {
        ManufacturingOrderId = manufacturingOrderId;
        MaterialDefinition = materialDefinition;
        CreatedBy = createdBy;
        Customer = customer;
        OperationsDefinition = operationsDefinition;
        QualityStandard = qualityStandard;
        Quantity = quantity;
        Unit = unit;
        DueDate = dueDate;
        AvailableDate = availableDate;
        CreateDate = createDate;
        WorkOrders = workOrders;
        Priority = priority;
        Status = status;
    }
}


