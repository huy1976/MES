namespace MesMicroservice.Application.Queries.ManufacturingRecords;

public class ManufacturingRecordViewModel
{
    public string WorkOrderId { get; private set; }
    public string OutputMaterialDefinitionId { get; private set; }
    public List<string> EquipmentIds { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public decimal Output { get; private set; }
    public decimal Defects { get; private set; }
    public decimal TotalOutput => Output + Defects;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ManufacturingRecordViewModel() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public ManufacturingRecordViewModel(string workOrderId, string outputMaterialDefinitionId, List<string> equipmentIds, DateTime startTime, DateTime endTime, decimal output, decimal defects)
    {
        WorkOrderId = workOrderId;
        OutputMaterialDefinitionId = outputMaterialDefinitionId;
        EquipmentIds = equipmentIds;
        StartTime = startTime;
        EndTime = endTime;
        Output = output;
        Defects = defects;
    }
}
