namespace MesMicroservice.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ManufacturingRecordsController : ApiControllerBase
{
    public ManufacturingRecordsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<IActionResult> CreateManufacturingRecord([FromBody] CreateManufacturingRecordCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] ManufacturingRecordsQuery query)
    {
        return await RequestAsync(query);
    }
}
