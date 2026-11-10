global using MesMicroservice.Application.DTOs.PlanForMaterialAndPersonnelDTOs;

namespace MesMicroservice.Application.Queries.PlanForMaterialAndPersonnels
{
    public class PlanForMaterialsQuery : IRequest<MaterialPlanResponseDto>
    {
        public DashboardTimeRange TimeRange { get; set; }

        public int? CustomMonth { get; set; }
        public int? CustomYear { get; set; }

        public PlanForMaterialsQuery(DashboardTimeRange timeRange, int? customMonth = null, int? customYear = null)
        {
            TimeRange = timeRange;
            CustomMonth = customMonth;
            CustomYear = customYear;
        }
    }
}
