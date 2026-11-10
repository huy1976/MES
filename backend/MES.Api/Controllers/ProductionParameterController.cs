namespace MesMicroservice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ProductionParameterController : ApiControllerBase
    {
        public ProductionParameterController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("CreateProductionParameter")]
        public async Task<IActionResult> CreateProductionParameter([FromBody] CreateProductionParameterCommand command)
        {
            return await RequestAsync(command);
        }

        [HttpPost("AddProductionParameterProperties")]
        public async Task<IActionResult> AddProductionParameterProperties([FromBody] AddProductionParameterPropertiesCommand command)
        {
            return await RequestAsync(command);
        }

        [HttpGet("GetProductionParameters")]
        public async Task<IActionResult> GetProductionParameters([FromQuery] ProductionParametersQuery query)
        {
            return await RequestAsync(query);
        }

        [HttpGet("GetProductionParameterPropertyNames")]
        public async Task<IActionResult> GetProductionParameterPropertyNames()
        {
            var query = new ProductionParameterPropertyNamesQuery();
            return await RequestAsync(query);
        }

        [HttpGet("GetProductionParameterIds")]
        public async Task<IActionResult> GetProductionParameterIds()
        {
            var query = new ProductionParameterIdsQuery();
            return await RequestAsync(query);
        }
    }
}
