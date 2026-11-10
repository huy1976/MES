namespace MesMicroservice.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EquipmentClassesController : ApiControllerBase
{
    public EquipmentClassesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<IActionResult> CreateEquipmentClass([FromBody] CreateEquipmentClassCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpPut]
    [Route("{equipmentClassId}")]
    public async Task<IActionResult> UpdateEquipmentClass([FromRoute] string equipmentClassId, [FromBody] UpdateEquipmentClassViewModel equipmentClass)
    {
        var command = new UpdateEquipmentClassCommand(equipmentClassId, equipmentClass.Name);
        return await RequestAsync(command);
    }

    [HttpGet]
    public async Task<IActionResult> GetEquipmentClasses([FromQuery] EquipmentClassesQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpGet("GetEquipmentClassPropertyNames")]
    public async Task<IActionResult> GetEquipmentClassPropertyNames()
    {
        var query = new EquipmentClassPropertyNamesQuery();
        return await RequestAsync(query);
    }

    [HttpGet("GetEquipmentClassIds")]
    public async Task<IActionResult> GetEquipmentClassIds()
    {
        var query = new EquipmentClassIdsQuery();
        return await RequestAsync(query);
    }

    [HttpDelete]
    [Route("{equipmentClassId}")]
    public async Task<IActionResult> DeleteEquipmentClass([FromRoute] string equipmentClassId)
    {
        var command = new DeleteEquipmentClassCommand(equipmentClassId);
        return await RequestAsync(command);
    }
}
