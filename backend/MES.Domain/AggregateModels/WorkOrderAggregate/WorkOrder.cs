global using MesMicroservice.Domain.AggregateModels.EquipmentAggregate;
global using MesMicroservice.Domain.AggregateModels.ManufacturingOrderAggregate;
global using MesMicroservice.Domain.AggregateModels.ManufacucturingRecordAggregate;

namespace MesMicroservice.Domain.AggregateModels.WorkOrderAggregate;
public class WorkOrder : Entity, IAggregateRoot
{
    public string WorkOrderId { get; private set; }
    public string SegmentId { get; private set; }
    public OperationsSegment OperationsSegment { get; private set; }
    public WorkCenter? WorkCenter { get; private set; }
    public int? SequenceNumber { get; set; }
    public MaterialDefinition MaterialDefinition { get; set;}
    public decimal ProductionTarget { get; private set; }
    public string Unit { get; private set; }
    public DateTime DueDate { get; private set; }
    public double Duration { get; private set; }
    public DateTime? StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public DateTime? ActuallyStartTime { get; private set; }
    public DateTime? ActuallyEndTime { get; private set; }
    public EStatus WorkOrderStatus { get; private set; }
    public string QualityStandardId { get; set; }
    public double SetupTime { get; set; }
    public double TeardownTime { get; set; }
    public double RunTime { get; set; }
    public bool? IsLateInSchedule => EndTime?.CompareTo(DueDate) > 0;
    public double? Lateness => (EndTime!.Value - DueDate).TotalHours < 0 ? 0 : (EndTime!.Value - DueDate).TotalHours;
    public int ManufacturingOrderId { get; private set; }
    public ManufacturingOrder ManufacturingOrder { get; private set; }
    public ICollection<ManufacturingRecord> ManufacturingRecords { get; private set; } = new List<ManufacturingRecord>();
    public ICollection<EquipmentWorkOrder> EquipmentWorkOrders { get; set; } = new List<EquipmentWorkOrder>();
    public ICollection<PersonWorkOrder> PersonWorkOrders { get; set; } = new List<PersonWorkOrder>();
    public ICollection<MaterialWorkOrder> MaterialWorkOrders { get; set; } = new List<MaterialWorkOrder>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private WorkOrder() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public WorkOrder(string workOrderId, string segmentId, OperationsSegment operationsSegment, WorkCenter? workCenter, int? sequenceNumber, 
                     MaterialDefinition materialDefinition, decimal productionTarget, string unit, DateTime dueDate, double duration, DateTime? startTime, 
                     DateTime? endTime, DateTime? actuallyStartTime, DateTime? actuallyEndTime, EStatus workOrderStatus, string qualityStandardId, 
                     double setupTime, double teardownTime, double runTime, int manufacturingOrderId, ManufacturingOrder manufacturingOrder, 
                     ICollection<ManufacturingRecord> manufacturingRecords, ICollection<EquipmentWorkOrder> equipmentWorkOrders, 
                     ICollection<PersonWorkOrder> personWorkOrders, ICollection<MaterialWorkOrder> materialWorkOrders)
    {
        WorkOrderId = workOrderId;
        SegmentId = segmentId;
        OperationsSegment = operationsSegment;
        WorkCenter = workCenter;
        SequenceNumber = sequenceNumber;
        MaterialDefinition = materialDefinition;
        ProductionTarget = productionTarget;
        Unit = unit;
        DueDate = dueDate;
        Duration = duration;
        StartTime = startTime;
        EndTime = endTime;
        ActuallyStartTime = actuallyStartTime;
        ActuallyEndTime = actuallyEndTime;
        WorkOrderStatus = workOrderStatus;
        QualityStandardId = qualityStandardId;
        SetupTime = setupTime;
        TeardownTime = teardownTime;
        RunTime = runTime;
        ManufacturingOrderId = manufacturingOrderId;
        ManufacturingOrder = manufacturingOrder;
        ManufacturingRecords = manufacturingRecords;
        EquipmentWorkOrders = equipmentWorkOrders;
        PersonWorkOrders = personWorkOrders;
        MaterialWorkOrders = materialWorkOrders;
    }

    public void UpdateProductionTarget(decimal newProductionTarget)
    {
        ProductionTarget = newProductionTarget;
    }

    public void UpdateWorkOrderStatus(EStatus newStatus)
    {
        WorkOrderStatus = newStatus;
    }

    public void UpdateDate(DateTime? start, DateTime? end, double duration, double runtime, double setuptime)
    {
        StartTime = start;
        EndTime = end;
        Duration = duration;
        RunTime = runtime;
        SetupTime = setuptime;
    }

    public void UpdateWorkCenter(WorkCenter? workCenter)
    {
        WorkCenter = workCenter;
    }

    public void UpdateSomeOrder(
    List<EquipmentWorkOrder> equipmentWorkOrders,
    List<PersonWorkOrder> personWorkOrders,
    List<MaterialWorkOrder> materialWorkOrders)
    {
        EquipmentWorkOrders.Clear();
        foreach (var item in equipmentWorkOrders)
            EquipmentWorkOrders.Add(item);

        PersonWorkOrders.Clear();
        foreach (var item in personWorkOrders)
            PersonWorkOrders.Add(item);

        MaterialWorkOrders.Clear();
        foreach (var item in materialWorkOrders)
            MaterialWorkOrders.Add(item);
    }

    public void UpdateStatus(EStatus status)
    {
        WorkOrderStatus = status;
    }

    public void UpdateActuallyTime(DateTime? actuallyStartTime, DateTime? actuallyEndTime)
    {
        ActuallyStartTime = actuallyStartTime;
        ActuallyEndTime = actuallyEndTime;
    }

    public void UpdatePlanTime(DateTime? start, DateTime? end)
    {
        StartTime = start;
        EndTime = end;
    }

    public void Delay(TimeSpan delay)
    {
        if (StartTime.HasValue && EndTime.HasValue)
            UpdatePlanTime(StartTime.Value.Add(delay), EndTime.Value.Add(delay));
    }



    //public void SetId(string workOrderId)
    //{
    //    WorkOrderId = workOrderId;
    //}

    //public void SetPrerequisiteOperations(List<WorkOrder> prerequisiteOperations)
    //{
    //    PrerequisiteOperations = prerequisiteOperations;
    //}

    //public void SetStartTime(DateTime startTime)
    //{
    //    StartTime = startTime;
    //    EndTime = startTime + Duration;
    //}

    //public void SetDuration(TimeSpan duration)
    //{
    //    Duration = duration;
    //    if (StartTime.HasValue)
    //    {
    //        EndTime = StartTime + Duration;
    //    }
    //}

    //public void SetDueDate(DateTime dueDate)
    //{
    //    DueDate = dueDate;
    //}

    //public void SetProductionTarget(decimal productionTarget)
    //{
    //    ProductionTarget = productionTarget;
    //}

    //public void SetManufacturingOrder(ManufacturingOrder manufacturingOrder)
    //{
    //    ManufacturingOrder = manufacturingOrder;
    //}

    //public void AddManufacturingRecord(ManufacturingRecord manufacturingRecord)
    //{
    //    ManufacturingRecords.Add(manufacturingRecord);

    //    var predictedCompletionTime = PredictCompletionTime();

    //    var predictedEvent = new WorkOrderCompletionTimePredictedEvent(this, predictedCompletionTime);
    //    AddDomainEvent(predictedEvent);
    //}

    public DateTime PredictCompletionTime()
    {
        //if (Progress > 0)
        //{
        //    var predictedDurationTicks = ((decimal)(DateTime.Now - ActuallyStartTime)!.Value.Ticks) / Progress;
        //    var predictedDuration = TimeSpan.FromTicks((long)predictedDurationTicks);
        //    return ActuallyStartTime!.Value + predictedDuration;
        //}
        //else
        //{
        //    return DateTime.Now + Duration;
        //}
        return DateTime.Now;
    }

    public void Update(double duration, DateTime? startTime, DateTime? endTime, DateTime? actuallyStartTime, DateTime? actuallyEndTime, EStatus workOrderStatus, WorkCenter? workCenter)
    {
        Duration = duration;
        StartTime = startTime;
        EndTime = endTime;
        ActuallyStartTime = actuallyStartTime;
        ActuallyEndTime = actuallyEndTime;
        WorkOrderStatus = workOrderStatus;
        WorkCenter = workCenter;
    }
}
