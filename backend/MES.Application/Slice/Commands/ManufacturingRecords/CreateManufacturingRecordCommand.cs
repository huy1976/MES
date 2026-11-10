namespace MesMicroservice.Application.Commands.ManufacturingRecords;

[DataContract]
public class CreateManufacturingRecordCommand : IRequest<bool>
{
    [DataMember]
    public string ManufacturingOrderId { get; set; }
    [DataMember]
    public string WorkOrderId { get; set; }
    [DataMember]
    public List<string> EquipmentIds { get; set; }
    [DataMember]
    public DateTime StartTime { get; set; }
    [DataMember]
    public DateTime EndTime { get; set; }
    [DataMember]
    public decimal Output { get; set; }
    [DataMember]
    public decimal Defects { get; set; }

    public CreateManufacturingRecordCommand(string manufacturingOrderId, string workOrderId, List<string> equipmentIds, DateTime startTime, DateTime endTime, decimal output, decimal defects)
    {
        ManufacturingOrderId = manufacturingOrderId;
        WorkOrderId = workOrderId;
        EquipmentIds = equipmentIds;
        StartTime = startTime;
        EndTime = endTime;
        Output = output;
        Defects = defects;
    }
}
