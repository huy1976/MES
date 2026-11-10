namespace MesMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[Authorize(Roles = "Admin, WarehouseManager")]
    public class PersonController : ApiControllerBase
    {
        public PersonController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("CreatePerson")]
        public async Task<IActionResult> CreatePerson([FromBody] CreatePersonCommand command)
        {
            return await RequestAsync(command);
        }

        [HttpPost("CreatePersons")]
        public async Task<IActionResult> CreatePersons([FromBody] CreatePersonsCommand command)
        {
            return await RequestAsync(command);
        }

        [HttpPost("CreatePersonnelCapability")]
        public async Task<IActionResult> CreatePersonnelCapability([FromBody] CreatePersonnelCapabilityCommand command)
        {
            return await RequestAsync(command);
        }

        [HttpPost("CreatePersonnelCapabilities")]
        public async Task<IActionResult> CreatePersonnelCapabilities([FromBody] CreatePersonnelCapabilitiesCommand command)
        {
            return await RequestAsync(command);
        }

        //[HttpPut]
        //[Route("{personId}")]
        //public async Task<IActionResult> UpdatePerson([FromRoute] string personId, [FromBody] UpdatePersonViewModel person)
        //{
        //    var command = new UpdatePersonCommand(personId, person.Name, person.Role);
        //    return await RequestAsync(command);
        //}

        [HttpGet]
        public async Task<IActionResult> GetPersons([FromQuery] PersonsQuery query)
        {
            return await RequestAsync(query);
        }

        [HttpGet("GetPersonPropertyNames")]
        public async Task<IActionResult> GetPersonPropertyNames()
        {
            var query = new PersonPropertyNamesQuery();
            return await RequestAsync(query);
        }

        [HttpGet("GetPersonnelCapabilities")]
        public async Task<IActionResult> GetPersonnelCapabilities([FromQuery] PersonnelCapabilitiesQuery query)
        {
            return await RequestAsync(query);
        }

        [HttpGet("GetPersonnelCapabilityPropertyNames")]
        public async Task<IActionResult> GetPersonnelCapabilityPropertyNames()
        {
            var query = new PersonnelCapabilityPropertyNamesQuery();
            return await RequestAsync(query);
        }

        [HttpGet("GetPersonIds")]
        public async Task<IActionResult> GetPersonIds()
        {
            var query = new PersonIdsQuery();
            return await RequestAsync(query);
        }

        //[HttpDelete]
        //[Route("{personId}")]
        //public async Task<IActionResult> DeletePerson([FromRoute] string personId)
        //{
        //    var command = new DeletePersonCommand(personId);
        //    return await RequestAsync(command);
        //}
    }
}
