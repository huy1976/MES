namespace MesMicroservice.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MaterialLotsController : ApiControllerBase
{
    public MaterialLotsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetMaterialLots([FromQuery] MaterialLotsQuery query)
    {
        return await RequestAsync(query);
    }
}
