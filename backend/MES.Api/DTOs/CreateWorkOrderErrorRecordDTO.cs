global using MesMicroservice.Domain.Enums;

namespace MesMicroservice.Api.DTOs
{
    public class CreateWorkOrderErrorRecordDTO
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
        public bool? IsUpdateTime { get; set; } = false;
        public bool? IsSchedule { get; set; } = false;
    }

}
