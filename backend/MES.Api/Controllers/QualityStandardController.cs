
global using MesMicroservice.Application.Commands.QualityStandards;
global using MesMicroservice.Application.Queries.QualityStandards;

namespace MesMicroservice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QualityStandardController : ApiControllerBase
    {
        public QualityStandardController(IMediator mediator) : base(mediator)
        {
        }   

        [HttpPost("CreateQualityStandard")]
        public async Task<IActionResult> CreateQualityStandard([FromBody] CreateQualityStandardCommand command)
        {
            return await RequestAsync(command);
        }

        [HttpGet("GetQualityStandardsByMaterialId/{materialId}")]
        public async Task<IActionResult> GetQualityStandardsByMaterialId(string materialId)
        {
            var query = new GetQualityStandardsByMaterialIdQuery(materialId);
            return await RequestAsync(query);
        }

        [HttpGet("GetQualityStandards")]
        public async Task<IActionResult> GetQualityStandards([FromQuery] QualityStandardsQuery query)
        {
            return await RequestAsync(query);
        }

        [HttpGet("GetQualityStandardPropertyNames")]
        public async Task<IActionResult> GetQualityStandardPropertyNames()
        {
            var query = new QualityStandardPropertyNamesQuery();
            return await RequestAsync(query);
        }

        [HttpPost("AddMaterialIntoQualityStandard")]
        public async Task<IActionResult> AddMaterialIntoQualityStandard([FromBody] AddMaterialIntoQualityStandardCommand command)
        {
            return await RequestAsync(command);
        }
    }
}
