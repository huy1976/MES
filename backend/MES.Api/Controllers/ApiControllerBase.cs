namespace MesMicroservice.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ApiControllerBase : ControllerBase
{
    protected readonly IMediator _mediator;

    public ApiControllerBase(IMediator mediator)
    {
        _mediator = mediator;
    }

    protected async Task<IActionResult> RequestAsync<T>(IRequest<T> request)
    {
        try
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        catch (ResourceNotFoundException ex)
        {
            var errorMessage = new ErrorMessage(ex);
            return NotFound(errorMessage);
        }
        catch (EntitiesNotFoundException ex)
        {
            var errorMessage = new ErrorMessage(ex);
            return BadRequest(errorMessage);
        }
        catch (WorkOrderNotScheduledException ex)
        {
            var errorMessage = new ErrorMessage(ex);
            return BadRequest(errorMessage);
        }
        catch (EquipmentOccupiedException ex)
        {
            var errorMessage = new ErrorMessage(ex);
            return BadRequest(errorMessage);
        }
        catch (EntityDuplicationException ex)
        {
            var errorMessage = new ErrorMessage(ex);
            return BadRequest(errorMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = new ErrorMessage(ex);
            return BadRequest(errorMessage);
        }
    }

    protected async Task<TResponse> RequestDtoAsync<TResponse>(IRequest<TResponse> request)
    {
        return await _mediator.Send(request);
    }
}
