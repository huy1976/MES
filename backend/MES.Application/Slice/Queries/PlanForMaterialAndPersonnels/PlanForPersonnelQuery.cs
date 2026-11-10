global using MesMicroservice.Application.Queries.Dashboards;
using MesMicroservice.Application.DTOs.PlanForPersonnelAndPersonnelDTOs;

namespace MesMicroservice.Application.Queries.PlanForMaterialAndPersonnels
{
    public class PlanForPersonnelQuery : IRequest<PersonnelPlanResponseDto>
    {
        public DashboardTimeRange TimeRange { get; set; }

        public int? CustomMonth { get; set; }
        public int? CustomYear { get; set; }

        public PlanForPersonnelQuery(DashboardTimeRange timeRange, int? customMonth = null, int? customYear = null)
        {
            TimeRange = timeRange;
            CustomMonth = customMonth;
            CustomYear = customYear;
        }
    }
}
