global using MesMicroservice.Application.Queries.BusinessPartners;
global using MesMicroservice.Application.Commands.BusinessPartners;

namespace MesMicroservice.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BusinessPartnerController : ApiControllerBase
{
    public BusinessPartnerController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<IActionResult> CreateBusinessPartner([FromBody] CreateBusinessPartnerCommand command)
    {
        return await RequestAsync(command);
    }


    [HttpGet("GetBusinessPartnerById/{businessPartnerId}")]

    public async Task<IActionResult?> GetBusinessPartnerById(string businessPartnerId)
    {
        var query = new BusinessPartnerQuery(businessPartnerId);

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

    [HttpGet("GetBusinessPartners")]
    public async Task<IActionResult> GetBusinessPartners([FromQuery] BusinessPartnersQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpGet("GetCustomerIds")]
    public async Task<IActionResult> GetCustomerIds()
    {
        var query = new CustomerIdsQuery();
        return await RequestAsync(query);
    }

    [HttpGet("GetBusinessPartnerPropertyNames")]
    public async Task<IActionResult> GetBusinessPartnerPropertyNames()
    {
        var query = new BusinessPartnerPropertyNamesQuery();
        return await RequestAsync(query);
    }


}
