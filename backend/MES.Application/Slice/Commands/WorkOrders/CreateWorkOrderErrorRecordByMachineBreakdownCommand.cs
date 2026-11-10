namespace MesMicroservice.Application.Commands.WorkOrders
{
    public class CreateWorkOrderErrorRecordByMachineBreakdownCommand : IRequest<bool>
    {
        public string ErrorId { get; set; } = string.Empty;
        public string ErrorName { get; set; } = string.Empty;
        public EDowntimeType DowntimeType { get; set; }
        public string? Notes { get; set; } = string.Empty;
        public string? HandledBy { get; set; } = string.Empty;
        public string ReportedBy { get; set; } = string.Empty;
        public DateTime Start { get; set; } = DateTime.MinValue;
        public DateTime End { get; set; } = DateTime.MinValue;
        public string WorkOrderId { get; set; } = string.Empty;
        public bool? IsUpdateTime { get; set; }
        public bool? IsSchedule { get; set; }
    }
}
