namespace MesMicroservice.Domain.AggregateModels.WorkOrderAggregate
{
    public class WorkOrderProgressRecord : IAggregateRoot
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public double ProgressPercentage { get; set; }
        public EStatus Status { get; set; }
        public double TotalQuantity { get; set; }
        public double ProducedQuantity { get; set; }
        public double ErrorQuantity { get; set; }
        public double DeltaQuantity { get; set; }
        public double DeltaErrorQuantity { get; set; }
        public double RawConsumedQuatity { get; set; }
        public double DeltaRawConsumedQuatity { get; set; }
        public string UnitOfMeasure { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public DateTime Timestamp { get; set; }
        public string Note { get; set; }

        // From DB
        public string Equipment { get; set; }
        public string WorkOrderId { get; set; }
        public string UpdatedBy { get; set; }

        public WorkOrderProgressRecord(double progressPercentage, EStatus status, double totalQuantity, double producedQuantity, double errorQuantity, 
                                       double deltaQuantity, double deltaErrorQuantity, double rawConsumedQuatity, double deltaRawConsumedQuatity, 
                                       string unitOfMeasure, DateTime? actualStartDate, DateTime? actualEndDate, DateTime timestamp, string note, 
                                       string equipment, string workOrderId, string updatedBy)
        {
            ProgressPercentage = progressPercentage;
            Status = status;
            TotalQuantity = totalQuantity;
            ProducedQuantity = producedQuantity;
            ErrorQuantity = errorQuantity;
            DeltaQuantity = deltaQuantity;
            DeltaErrorQuantity = deltaErrorQuantity;
            RawConsumedQuatity = rawConsumedQuatity;
            DeltaRawConsumedQuatity = deltaRawConsumedQuatity;
            UnitOfMeasure = unitOfMeasure;
            ActualStartDate = actualStartDate;
            ActualEndDate = actualEndDate;
            Timestamp = timestamp;
            Note = note;
            Equipment = equipment;
            WorkOrderId = workOrderId;
            UpdatedBy = updatedBy;
        }




#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public WorkOrderProgressRecord() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.





    }
}
