global using MesMicroservice.Domain.AggregateModels.BusinessPartnerAggregate;
global using MesMicroservice.Domain.AggregateModels.OperationAggregate;
global using MesMicroservice.Domain.AggregateModels.WorkOrderAggregate;

namespace MesMicroservice.Domain.AggregateModels.ManufacturingOrderAggregate;
public class ManufacturingOrder : Entity, IAggregateRoot
{
    public string ManufacturingOrderId { get; private set; }
    public MaterialDefinition MaterialDefinition { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; }
    public DateTime AvailableDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime PlanedStartDate { get; set; }   
    public DateTime PlanedEnDate { get; set; }
    public BusinessPartner Customer { get; set; }
    public OperationsDefinition OperationsDefinition { get; set; }
    public QualityStandard QualityStandard { get; set; }
    public EStatus Status { get; set; }
    public DateTime CreateDate { get; private set; } = DateTime.UtcNow;
    public Person CreatedBy { get; private set; }
    public int Priority { get; private set; }

    public ICollection<WorkOrder> WorkOrders { get; private set; } = new List<WorkOrder>();


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ManufacturingOrder() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public ManufacturingOrder(string manufacturingOrderId, MaterialDefinition materialDefinition, decimal quantity, string unit, DateTime availableDate, DateTime dueDate, DateTime planedStartDate, DateTime planedEnDate, BusinessPartner customer, OperationsDefinition operationsDefinition, QualityStandard qualityStandard, EStatus status, DateTime createDate, Person createdBy, int priority, ICollection<WorkOrder> workOrders)
    {
        ManufacturingOrderId = manufacturingOrderId;
        MaterialDefinition = materialDefinition;
        Quantity = quantity;
        Unit = unit;
        AvailableDate = availableDate;
        DueDate = dueDate;
        PlanedStartDate = planedStartDate;
        PlanedEnDate = planedEnDate;
        Customer = customer;
        OperationsDefinition = operationsDefinition;
        QualityStandard = qualityStandard;
        Status = status;
        CreateDate = createDate;
        CreatedBy = createdBy;
        Priority = priority;
        WorkOrders = workOrders;
    }

    public void Update(MaterialDefinition materialDefinition, decimal quantity, string unit, DateTime dueDate, DateTime availableDate, int priority)
    {
        MaterialDefinition = materialDefinition;
        Quantity = quantity;
        Unit = unit;
        DueDate = dueDate;
        AvailableDate = availableDate;
        Priority = priority;
    }

    public void AddWorkOrder(WorkOrder workOrder)
    {
        if (WorkOrders.Any(d => d.WorkOrderId == workOrder.WorkOrderId))
        {
            throw new ChildEntityDuplicationException(workOrder.WorkOrderId, workOrder, ManufacturingOrderId, this);
        }

        WorkOrders.Add(workOrder);
    }
    public void UpdateStatus(EStatus status)
    {
        Status = status;
    }

    public void UpdateDates(DateTime planedStartDate, DateTime planedEndDate)
    {
        PlanedStartDate = planedStartDate;
        PlanedEnDate = planedEndDate;
    }

    //public WorkOrder GetWorkOrderWithId(string workOrderId)
    //{
    //    var workOrder = WorkOrders.Find(d => d.WorkOrderId == workOrderId);
    //    return workOrder ?? throw new ChildEntityNotFoundException(workOrderId, typeof(WorkOrder), ManufacturingOrderId, this);
    //}

    //public void RemoveWorkOrder(string workOrderId)
    //{
    //    var workOrder = GetWorkOrderWithId(workOrderId);

    //    try
    //    {
    //        WorkOrders.Remove(workOrder);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new DomainException($"ManufacturingOrder with id {ManufacturingOrderId} throw an exception. See inner exception for details.", ex);
    //    }
    //}
}
