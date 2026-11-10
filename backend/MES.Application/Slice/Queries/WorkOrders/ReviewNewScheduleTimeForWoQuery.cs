namespace MesMicroservice.Application.Queries.WorkOrders
{
    public class ReviewNewScheduleTimeForWoQuery : IRequest<List<ReScheduleTimeForWODTO>>
    {
        public string WoId { get; set; } = string.Empty;
        public double Duration { get; set; }
    }
}
