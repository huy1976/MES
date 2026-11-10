global using MesMicroservice.Application.DTOs.DashboardDTOs;

namespace MesMicroservice.Application.Queries.Dashboards;

public class DashboardQuery : IRequest<DashboardDTO>
{
    public DashboardTimeRange TimeRange { get; set; }

    public DashboardQuery(DashboardTimeRange timeRange)
    {
        TimeRange = timeRange;
    }
}

public enum DashboardTimeRange
{
    Day,
    Week,
    Month,
    MonthCustom,
    All
}

