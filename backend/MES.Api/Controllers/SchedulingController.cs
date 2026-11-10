
global using MesMicroservice.Application.Commands.Scheduling;
using MesMicroservice.Application.Queries.Scheduling;

namespace MesMicroservice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class SchedulingController : ApiControllerBase
    {
        public SchedulingController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("RunScheduling")]
        public async Task<IActionResult> RunScheduling([FromBody] SchedulingForWorkOrdersCommand schedule)
        {
            return await RequestAsync(schedule);
        }

        [HttpPost("ResultSchedulingForWorkOrders")]
        public async Task<IActionResult> ResultSchedulingForWorkOrders([FromBody] ResultSchedulingForWorkOrdersCommand schedule)
        {
            return await RequestAsync(schedule);
        }
        [HttpGet("CurrentProductionSchedule")]
        public async Task<IActionResult> CurrentProductionSchedule()
        {
            return await RequestAsync(new CurrentProductionScheduleQuery());
        }
        [HttpPost("RescheduleUrgentOrder")]
        public async Task<IActionResult> RescheduleUrgentOrder([FromBody] RescheduleUrgentOrderCommand command)
        {
            return await RequestAsync(command);
        }
    }
}
