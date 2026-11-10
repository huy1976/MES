namespace MesMicroservice.Application.DTOs.DashboardDTOs
{
    public class MOReportDTO
    {
        public int TotalMOs { get; set; }
        public int PendingMOs { get; set; }
        public int PendingScheduledMOs { get; set; }
        public int ReadyToProduceMOs { get; set; }
        public int InProgressMOs { get; set; }
        public int CompletedMOs { get; set; }

        public int CancelledMOs { get; set; }
        public double CompletionRate { get; set; }
    }
}
