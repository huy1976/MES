using MesMicroservice.Domain.Enums;

namespace MesMicroservice.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ManufacturingOrdersController : ApiControllerBase
{
    public ManufacturingOrdersController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [Authorize(Policy = "EditPage2Policy")]
    public async Task<IActionResult> CreateManufacturingOrder([FromBody] CreateManufacturingOrderCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpPut]
    [Route("{manufacturingOrderId}")]
    public async Task<IActionResult> UpdateManufacturingOrder([FromRoute] string manufacturingOrderId, [FromBody] UpdateManufacturingOrderViewModel manufacturingOrder)
    {
        var command = new UpdateManufacturingOrderCommand(manufacturingOrderId, manufacturingOrder.MaterialDefinitionId, manufacturingOrder.Quantity, manufacturingOrder.Unit, manufacturingOrder.DueDate, manufacturingOrder.AvailableDate, manufacturingOrder.Priority);
        return await RequestAsync(command);
    }

    [HttpPatch("UpdateStatusForManufacturingOrders")]
    public async Task<IActionResult> UpdateStatusForManufacturingOrders([FromBody] UpdateStatusForManufacturingOrdersCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpGet]
    public async Task<IActionResult> GetManufacturingOrders([FromQuery] ManufacturingOrdersQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpGet("GetManufacturingOrdersByStatus{status}")]
    public async Task<IActionResult> GetManufacturingOrdersByStatus([FromRoute] EStatus status)
    {
        var query = new ManufacturingOrdersByStatusQuery(status);
        return await RequestAsync(query);
    }


    [HttpDelete]
    [Route("{manufacturingOrderId}")]
    public async Task<IActionResult> DeleteManufacturingOrder([FromRoute] string manufacturingOrderId)
    {
        var command = new DeleteManufacturingOrderCommand(manufacturingOrderId);
        return await RequestAsync(command);
    }

    [HttpGet]
    [Route("{manufacturingOrderId}")]
    public async Task<IActionResult> GetManufacturingOrder([FromRoute] string manufacturingOrderId)
    {
        var query = new ManufacturingOrderQuery(manufacturingOrderId);
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
}
