namespace MesMicroservice.Application.DTOs.WorkOrderDTOs;

public class WorkOrderErrorRecordDTO
{
    public string Id { get; set; } 
    public string ErrorId { get; set; }
    public string ErrorName { get;  set; } 
    public EStatus Status { get;  set; }
    public string DowntimeType { get;  set; }
    public string Notes { get;  set; } 
    public string HandledBy { get;  set; } 
    public string ReportedBy { get;  set; } 
    public DateTime Timestamp { get;  set; }
    public double DurationInHours { get;  set; }
    public DateTime Start { get;  set; } 
    public DateTime End { get;  set; }
    public string WorkOrderId { get;  set; }
    public bool IsUpdateTime { get;  set; }
    public bool IsSchedule { get;  set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public WorkOrderErrorRecordDTO()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public WorkOrderErrorRecordDTO(string id, string errorId, string errorName, EStatus status, string downtimeType, string notes, string handledBy, string reportedBy, DateTime timestamp, double durationInHours, DateTime start, DateTime end, string workOrderId, bool isUpdateTime, bool isSchedule)
    {
        Id = id;
        ErrorId = errorId;
        ErrorName = errorName;
        Status = status;
        DowntimeType = downtimeType;
        Notes = notes;
        HandledBy = handledBy;
        ReportedBy = reportedBy;
        Timestamp = timestamp;
        DurationInHours = durationInHours;
        Start = start;
        End = end;
        WorkOrderId = workOrderId;
        IsUpdateTime = isUpdateTime;
        IsSchedule = isSchedule;
    }
}
