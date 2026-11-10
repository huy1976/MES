global using MesMicroservice.Application.Commands.SegmentModels;
using MesMicroservice.Application.Queries.SegmentModels;

namespace MesMicroservice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ProcessSegmentController : ApiControllerBase
    {
        public ProcessSegmentController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("CreateProcessSegment")]
        public async Task<IActionResult> CreateProcessSegment([FromBody] CreateProcessSegmentCommand command)
        {
            return await RequestAsync(command);
        }

        [HttpPost("AddMaterialSegmentSpecifications")]
        public async Task<IActionResult> AddMaterialSegmentSpecifications([FromBody] AddMaterialSegmentSpecificationsCommand command)
        {
            return await RequestAsync(command);
        }


        [HttpGet("GetProcessSegmentById/{processSegmentId}")]
        public async Task<IActionResult> GetProcessSegmentById(string processSegmentId)
        {
            var query = new ProcessSegmentQuery(processSegmentId);
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

        [HttpGet("GetProcessSegments")]
        public async Task<IActionResult> GetProcessSegments([FromQuery] ProcessSegmentsQuery query)
        {
            return await RequestAsync(query);
        }
        [HttpPost("AddEquipmentSegmentSpecifications")]
        public async Task<IActionResult> AddEquipmentSegmentSpecifications([FromBody] AddEquipmentSegmentSpecificationsCommand command)
        {
            return await RequestAsync(command);
        }

        [HttpPost("AddPersonnelSegmentSpecifications")]
        public async Task<IActionResult> AddPersonnelSegmentSpecifications([FromBody] AddPersonnelSegmentSpecificationsCommand command)
        {
            return await RequestAsync(command);
        }

        [HttpGet("GetProcessSegmentIds")]
        public async Task<IActionResult> GetProcessSegmentIds()
        {
            var query = new ProcessSegmentIdsQuery();
            return await RequestAsync(query);
        }

        [HttpGet("GetMaterialSegmentSpecifications")]
        public async Task<IActionResult> GetMaterialSegmentSpecifications([FromQuery] MaterialSegmentSpecificationsQuery query)
        {
            return await RequestAsync(query);
        }

        [HttpGet("GetMaterialSegmentSpecificationPropertyNames")]
        public async Task<IActionResult> GetMaterialSegmentSpecificationPropertyNames()
        {
            var query = new MaterialSegmentSpecificationPropertyNamesQuery();
            return await RequestAsync(query);
        }

        [HttpGet("GetPersonnelSegmentSpecifications")]
        public async Task<IActionResult> GetPersonnelSegmentSpecifications([FromQuery] PersonnelSegmentSpecificationsQuery query)
        {
            return await RequestAsync(query);
        }

        [HttpGet("GetPersonnelSegmentSpecificationPropertyNames")]
        public async Task<IActionResult> GetPersonnelSegmentSpecificationPropertyNames()
        {
            var query = new PersonnelSegmentSpecificationPropertyNamesQuery();
            return await RequestAsync(query);
        }

        [HttpGet("GetEquipmentSegmentSpecifications")]
        public async Task<IActionResult> GetEquipmentSegmentSpecifications([FromQuery] EquipmentSegmentSpecificationsQuery query)
        {
            return await RequestAsync(query);
        }

        [HttpGet("GetEquipmentSegmentSpecificationPropertyNames")]
        public async Task<IActionResult> GetEquipmentSegmentSpecificationPropertyNames()
        {
            var query = new EquipmentSegmentSpecificationPropertyNamesQuery();
            return await RequestAsync(query);
        }

        [HttpGet("GetEquipmentSegmentSpecificationsByProcessSegmentId/{processSegmentId}")]
        public async Task<IActionResult> GetEquipmentSegmentSpecificationsByProcessSegmentId([FromRoute] string processSegmentId)
        {
            var query = new EquipmentSegmentSpecificationsByProcessSegmentIdQuery(processSegmentId);
            return await RequestAsync(query);
        }

        [HttpGet("GetMaterialSegmentSpecificationsByProcessSegmentId/{processSegmentId}")]
        public async Task<IActionResult> GetMaterialSegmentSpecificationsByProcessSegmentId([FromRoute] string processSegmentId)
        {
            var query = new MaterialSegmentSpecificationsByProcessSegmentIdQuery(processSegmentId);
            return await RequestAsync(query);
        }

        [HttpGet("GetPersonnelSegmentSpecificationsByProcessSegmentId/{processSegmentId}")]
        public async Task<IActionResult> GetPersonnelSegmentSpecificationsByProcessSegmentId([FromRoute] string processSegmentId)
        {
            var query = new GetPersonnelSegmentSpecificationsByProcessSegmentIdQuery(processSegmentId);
            return await RequestAsync(query);
        }
    }
}
