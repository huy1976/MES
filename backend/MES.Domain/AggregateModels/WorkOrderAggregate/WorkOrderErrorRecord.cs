namespace MesMicroservice.Domain.AggregateModels.WorkOrderAggregate
{
    public class WorkOrderErrorRecord : IAggregateRoot
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public string ErrorId { get; private set; } = string.Empty;
        public string ErrorName { get; private set; } = string.Empty;
        public EStatus Status { get; private set; } = EStatus.Pending;
        public EDowntimeType DowntimeType { get; private set; }
        public string Notes { get; private set; } = string.Empty;
        public string HandledBy { get; private set; } = string.Empty;
        public string ReportedBy { get; private set; } = string.Empty;
        public DateTime Timestamp { get; private set; }
        public double DurationInHours { get; private set; }
        public DateTime Start { get; private set; } = DateTime.MinValue;
        public DateTime End { get; private set; } = DateTime.MinValue;
        public string WorkOrderId { get; private set; }
        public bool IsUpdateTime { get; private set; }
        public bool IsSchedule { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private WorkOrderErrorRecord() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public WorkOrderErrorRecord(string errorId, string errorName, EStatus status, EDowntimeType downtimeType, string notes, string handledBy, string reportedBy, DateTime timestamp, double durationInHours, DateTime start, DateTime end, string workOrderId, bool isUpdateTime, bool isSchedule)
        {
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
}
