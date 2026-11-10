
global using MesMicroservice.Application.Commands.OperationsDefinitions;
global using MesMicroservice.Application.Queries.OperationsDefinitions;

namespace MesMicroservice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OperationsDefinitionController : ApiControllerBase
    {
        public OperationsDefinitionController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("CreateOperationsDefinition")]
        public async Task<IActionResult> CreateOperationsDefinition([FromBody] CreateOperationsDefinitionCommand command)
        {
            return await RequestAsync(command);
        }

        [HttpPost("UpdateSpecifications")]
        public async Task<IActionResult> UpdateSpecifications([FromBody] UpdateSpecificationsCommand command)
        {
            return await RequestAsync(command);
        }

        [HttpPost("CreateOperationSegmentDependency")]
        public async Task<IActionResult> CreateOperationSegmentDependency([FromBody] CreateOperationSegmentDependencyCommand command)
        {
            return await RequestAsync(command);
        }

        [HttpGet("GetOperationsDefinitionsByMaterialId/{*materialId}")]
        public async Task<IActionResult> GetOperationsDefinitionsByMaterialId(string materialId)
        {
            var query = new GetOperationsDefinitionsByMaterialIdQuery(materialId);
            return await RequestAsync(query);
        }


        [HttpGet("GetOperationsDefinitions")]
        public async Task<IActionResult> GetOperationsDefinitions([FromQuery] OperationDefinitionsQuery query)
        {
            return await RequestAsync(query);
        }

        [HttpGet("GetOperationsDefinitionById/{operationId}")]
        public async Task<IActionResult?> GetOperationsDefinitionById([FromRoute] string operationId)
        {
            var query = new GetOperationsDefinitionByIdQuery(operationId);
            return await RequestAsync(query);
        }

    }
}
