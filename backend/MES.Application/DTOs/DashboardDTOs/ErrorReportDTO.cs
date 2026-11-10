namespace MesMicroservice.Application.DTOs.DashboardDTOs
{
    public class ErrorReportDTO
    {
        public int TotalMOs { get; set; }
        public List<ListErrorInMO> ErrorsInMOs { get; set; } = new List<ListErrorInMO>();
    }

    public class ListErrorInMO
    {
        public string MOId { get; set; } = string.Empty;
        public string WOId { get; set; } = string.Empty;
        public string ErrorType { get; set; } = string.Empty;
        public int ErrorCount { get; set; } = 0;
    }
}
