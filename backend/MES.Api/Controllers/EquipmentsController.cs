using MesMicroservice.Application.Queries.EquipmentCapabilities;

namespace MesMicroservice.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
//[Authorize]
//[Authorize(Roles = "Admin, WarehouseManager")]
public class EquipmentsController : ApiControllerBase
{
    public EquipmentsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("CreateEquipment")]
    public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpPost("AddEquipmentProperties")]
    public async Task<IActionResult> AddEquipmentProperties([FromBody] AddEquipmentPropertiesCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpPost("CreateEquipmentCapability")]
    public async Task<IActionResult> CreateEquipmentCapability([FromBody] CreateEquipmentCapabilityCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpPost("AddEquipmentCapabilityProperties")]
    public async Task<IActionResult> AddEquipmentCapabilityProperties([FromBody] AddEquipmentCapabilityPropertiesCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpPut]
    [Route("{equipmentId}")]
    public async Task<IActionResult> UpdateEquipment([FromRoute] string equipmentId, [FromBody] UpdateEquipmentViewModel equipment)
    {
        var command = new UpdateEquipmentCommand(equipmentId, equipment.Name, equipment.Properties, equipment.EquipmentClass, equipment.AbsolutePath);
        return await RequestAsync(command);
    }

    [HttpGet]
    public async Task<IActionResult> GetEquipments([FromQuery] EquipmentsQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpGet("GetEquipmentPropertyNames")]
    public async Task<IActionResult> GetEquipmentPropertyNames()
    {
        var query = new EquipmentPropertyNamesQuery();
        return await RequestAsync(query);
    }

    [HttpDelete]
    [Route("{equipmentId}")]
    public async Task<IActionResult> DeleteEquipment([FromRoute] string equipmentId)
    {
        var command = new DeleteEquipmentCommand(equipmentId);
        return await RequestAsync(command);
    }

    [HttpGet]
    [Route("{equipmentId}")]
    public async Task<IActionResult> GetEquipmentById([FromRoute]string equipmentId)
    {
        var query = new EquipmentQuery(equipmentId);
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

    [HttpGet("GetOees")]
    public async Task<IActionResult> GetOees([FromQuery] OeeQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpGet("GetEquipmentIds")]
    public async Task<IActionResult> GetEquipmentIds()
    {
        var query = new EquipmentIdQuery();
        return await RequestAsync(query);
    }

    [HttpGet("GetEquipmentCapabilities")]
    public async Task<IActionResult> GetEquipmentCapabilities([FromQuery] EquipmentCapabilitiesQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpGet("GetEquipmentCapabilityPropertyNames")]
    public async Task<IActionResult> GetEquipmentCapabilityPropertyNames()
    {
        var query = new EquipmentCapabilityPropertyNamesQuery();
        return await RequestAsync(query);
    }

    [HttpGet("GetEquipmentCapabilityUnits")]
    public async Task<IActionResult> GetEquipmentCapabilityUnits()
    {
        var query = new EquipmentCapabilityUnitsQuery();
        return await RequestAsync(query);
    }

    [HttpGet("GetEquipmentCapabilityPropertyUnits")]
    public async Task<IActionResult> GetEquipmentCapabilityPropertyUnits()
    {
        var query = new EquipmentCapabilityPropertyUnitsQuery();
        return await RequestAsync(query);
    }

    [HttpPatch("UpdateEquipmentStatuses")]
    public async Task<IActionResult> UpdateEquipmentStatuses([FromBody] UpdateEquipmentStatusCommand command)
    {
        return await RequestAsync(command);
    }
}
