
using MesMicroservice.Application.Queries.Dashboards;
using MesMicroservice.Application.Queries.PlanForMaterialAndPersonnels;

namespace MesMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PlanMaterialAndPersonnelController : ApiControllerBase
    {
        public PlanMaterialAndPersonnelController(IMediator mediator) : base(mediator)
        {
        }
        [HttpGet("GetPlanMaterials")]
        public async Task<IActionResult> GetPlanMaterials(
            [FromQuery] DashboardTimeRange timeRange = DashboardTimeRange.Month,
            [FromQuery] int? customMonth = null,
            [FromQuery] int? customYear = null)
        {
            var query = new PlanForMaterialsQuery(timeRange, customMonth, customYear);
            return await RequestAsync(query);
        }

        [HttpGet("GetPlanPersonnels")]
        public async Task<IActionResult> GetPlanPersonnels(
            [FromQuery] DashboardTimeRange timeRange = DashboardTimeRange.Month,
            [FromQuery] int? customMonth = null,
            [FromQuery] int? customYear = null)
        {
            var query = new PlanForPersonnelQuery(timeRange, customMonth, customYear);
            return await RequestAsync(query);
        }


    }
}
