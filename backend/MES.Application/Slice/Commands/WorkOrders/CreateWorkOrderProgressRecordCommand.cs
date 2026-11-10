namespace MesMicroservice.Application.Commands.WorkOrders
{
    public class CreateWorkOrderProgressRecordCommand : IRequest<bool>
    {
        public string WorkOrderId { get; set; } = string.Empty;
        public DateTime? Timestamp { get; set; } = DateTime.Now;
        public double DeltaQuantityProduced { get; set; }
        public double DeltaQuantityError { get; set; }
        public double DeltaRawConsumedQuatity { get; set; }
        public string RecordedBy { get; set; } = string.Empty;

        public CreateWorkOrderProgressRecordCommand(string workOrderId, DateTime? timestamp, double deltaQuantityProduced, double deltaQuantityError, double deltaRawConsumedQuatity, string recordedBy)
        {
            WorkOrderId = workOrderId;
            Timestamp = timestamp;
            DeltaQuantityProduced = deltaQuantityProduced;
            DeltaQuantityError = deltaQuantityError;
            DeltaRawConsumedQuatity = deltaRawConsumedQuatity;
            RecordedBy = recordedBy;
        }
    }
}
