namespace MesMicroservice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BillOfMaterialController : ApiControllerBase
{
    public BillOfMaterialController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("CreateBillOfMaterial")]
    public async Task<IActionResult> CreateBillOfMaterial([FromBody] CreateBillOfMaterialCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpGet("GetBillOfMaterialById/{billOfMaterialId}")]
    public async Task<IActionResult> GetBillOfMaterialById(string billOfMaterialId)
    {
        var query = new BillOfMaterialQuery(billOfMaterialId);
        var result = await RequestAsync(query);
        if (result is null)
        {
            return NotFound();
        }
        else
        {
            return Ok(result);
        }
    }

    [HttpGet("GetBillOfMaterials")]
    public async Task<IActionResult> GetBillOfMaterials([FromQuery] BillOfMaterialsQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpGet("GetBillOfMaterialIds")]
    public async Task<IActionResult> GetBillOfMaterialIds()
    {
        var query = new BillOfMaterialIdsQuery();
        return await RequestAsync(query);
    }
}
