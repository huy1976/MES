using MesMicroservice.Application.DTOs.DashboardDTOs;
using MesMicroservice.Application.Queries.Dashboards;

namespace MesMicroservice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ApiControllerBase
    {
        public DashboardController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("GetMOReport")]
        public async Task<IActionResult> GetMOReport([FromQuery] DashboardTimeRange timeRange = DashboardTimeRange.Month)
        {
            var query = new DashboardQuery(timeRange);
            return await RequestAsync(query);
        }

    }
}
