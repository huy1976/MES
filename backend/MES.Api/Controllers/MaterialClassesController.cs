namespace MesMicroservice.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MaterialClassesController : ApiControllerBase
{
    public MaterialClassesController(IMediator mediator) : base(mediator)
    { 
    }

    [HttpPost]
    public async Task<IActionResult> CreateMaterialClass([FromBody] CreateMaterialClassCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpPost("CreateMaterialClassProperties")]
    public async Task<IActionResult> AddMaterialClassProperties([FromBody] AddMaterialClassPropertiesCommand command)
    {
        return await RequestAsync(command);
    }


    [HttpPut]
    [Route("{materialClassId}")]
    public async Task<IActionResult> UpdateMaterialClass([FromRoute] string materialClassId, [FromBody] UpdateMaterialClassViewModel materialClass)
    {
        var command = new UpdateMaterialClassCommand(materialClassId, materialClass.Name);
        return await RequestAsync(command);
    }

    [HttpGet]
    public async Task<IActionResult> GetMaterialClasses([FromQuery] MaterialClassesQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpGet("GetMaterialClassPropertyNames")]
    public async Task<IActionResult> GetMaterialClassPropertyNames()
    {
        var query = new MaterialClassPropertyNamesQuery();
        return await RequestAsync(query);
    }

    [HttpGet("GetMaterialClassIds")]
    public async Task<IActionResult> GetMaterialClassIds()
    {
        var query = new MaterialClassIdsQuery();
        return await RequestAsync(query);
    }

    [HttpDelete]
    [Route("{materialClassId}")]
    public async Task<IActionResult> DeleteMaterialClass([FromRoute] string materialClassId)
    {
        var command = new DeleteMaterialClassCommand(materialClassId);
        return await RequestAsync(command);
    }
}
