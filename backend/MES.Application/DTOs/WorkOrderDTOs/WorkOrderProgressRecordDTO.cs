namespace MesMicroservice.Application.DTOs.WorkOrderDTOs;

public class WorkOrderProgressRecordDTO
{
    public string Id { get; set; } 
    public double ProgressPercentage { get; set; }
    public EStatus Status { get; set; }
    public double TotalQuantity { get; set; }
    public double ProducedQuantity { get; set; }
    public double ErrorQuantity { get; set; }
    public double DeltaQuantity { get; set; }
    public double DeltaErrorQuantity { get; set; }
    public string UnitOfMeasure { get; set; }
    public DateTime Timestamp { get; set; }
    public string Note { get; set; }
    public string Equipment { get; set; }
    public string WorkOrderId { get; set; }
    public string UpdatedBy { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public WorkOrderProgressRecordDTO()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public WorkOrderProgressRecordDTO(string id, double progressPercentage, EStatus status, double totalQuantity, double producedQuantity, double errorQuantity, double deltaQuantity, double deltaErrorQuantity, string unitOfMeasure, DateTime timestamp, string note, string equipment, string workOrderId, string updatedBy)
    {
        Id = id;
        ProgressPercentage = progressPercentage;
        Status = status;
        TotalQuantity = totalQuantity;
        ProducedQuantity = producedQuantity;
        ErrorQuantity = errorQuantity;
        DeltaQuantity = deltaQuantity;
        DeltaErrorQuantity = deltaErrorQuantity;
        UnitOfMeasure = unitOfMeasure;
        Timestamp = timestamp;
        Note = note;
        Equipment = equipment;
        WorkOrderId = workOrderId;
        UpdatedBy = updatedBy;
    }
}
