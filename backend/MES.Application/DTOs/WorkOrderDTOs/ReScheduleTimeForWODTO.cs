namespace MesMicroservice.Application.DTOs.WorkOrderDTOs
{
    public class ReScheduleTimeForWODTO
    {
        public string WorkOrderId { get; set; } = string.Empty;
        public DateTime NewStart { get; set; }
        public DateTime NewEnd { get; set; }
        public DateTime OldStart { get; set; }
        public DateTime OldEnd { get; set; }

        public ReScheduleTimeForWODTO()
        {
        }

        public ReScheduleTimeForWODTO(string workOrderId, DateTime newStart, DateTime newEnd, DateTime oldStart, DateTime oldEnd)
        {
            WorkOrderId = workOrderId;
            NewStart = newStart;
            NewEnd = newEnd;
            OldStart = oldStart;
            OldEnd = oldEnd;
        }
        public void SetOldTime(DateTime start, DateTime end) 
        {
            OldStart = start;
            OldEnd = end;
        }
    }
}
