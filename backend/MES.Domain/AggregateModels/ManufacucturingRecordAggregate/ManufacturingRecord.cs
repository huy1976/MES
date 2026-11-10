using MesMicroservice.Domain.AggregateModels.EquipmentAggregate;
using MesMicroservice.Domain.AggregateModels.WorkOrderAggregate;

namespace MesMicroservice.Domain.AggregateModels.ManufacucturingRecordAggregate;
public class ManufacturingRecord : Entity, IAggregateRoot
{
    public int WorkOrderId { get; private set; }
    public WorkOrder WorkOrder { get; private set; }
    public MaterialDefinition OutputMaterialDefinition { get; private set; }
    public List<Equipment> Equipments { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public decimal Output { get; private set; }
    public decimal Defects { get; private set; }
    public decimal RawOutput => Output + Defects;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ManufacturingRecord() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public ManufacturingRecord(WorkOrder workOrder, MaterialDefinition outputMaterialDefinition, List<Equipment> equipments, DateTime startTime, DateTime endTime, decimal output, decimal defects)
    {
        WorkOrder = workOrder;
        WorkOrderId = workOrder.Id;
        OutputMaterialDefinition = outputMaterialDefinition;
        Equipments = equipments;
        StartTime = startTime;
        EndTime = endTime;
        Output = output;
        Defects = defects;
    }
}
