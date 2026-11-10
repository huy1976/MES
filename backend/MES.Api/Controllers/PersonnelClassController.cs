namespace MesMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[Authorize(Roles = "Admin, WarehouseManager")]
    public class PersonnelClassController : ApiControllerBase
    {
        public PersonnelClassController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("CreatePersonnelClass")]
        public async Task<IActionResult> CreatePersonnelClass([FromBody] CreatePersonnelClassCommand command)
        {
            return await RequestAsync(command);
        }

        [HttpGet("GetPersonnelClasses")]
        public async Task<IActionResult> GetPersonnelClasses([FromQuery] PersonnelClassesQuery query)
        {
            return await RequestAsync(query);
        }

        [HttpGet("GetPersonnelClassPropertyNames")]
        public async Task<IActionResult> GetPersonnelClassPropertyNames()
        {
            var query = new PersonnelClassPropertyNamesQuery();
            return await RequestAsync(query);
        }

        [HttpGet("GetPersonnelClassIds")]
        public async Task<IActionResult> GetPersonnelClassIds()
        {
            var query = new PersonnelClassIdsQuery();
            return await RequestAsync(query);
        }
    }
}
