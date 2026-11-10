namespace MesMicroservice.Application.Queries.ManufacturingRecords;

public class ManufacturingRecordsQuery : PaginatedQuery, IRequest<IEnumrable<ManufacturingRecordViewModel>>
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? EquipmentId { get; set; }
}
