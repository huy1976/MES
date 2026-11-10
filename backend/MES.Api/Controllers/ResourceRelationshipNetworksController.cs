namespace MesMicroservice.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ResourceRelationshipNetworksController : ApiControllerBase
{
    public ResourceRelationshipNetworksController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<IActionResult> CreateResourceRelationshipNetwork([FromBody] CreateResourceRelationshipNetworkCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpPut]
    [Route("{resourceRelationshipNetworkId}")]
    public async Task<IActionResult> UpdateResourceRelationshipNetwork(string resourceRelationshipNetworkId, [FromBody] UpdateResourceRelationshipNetworkViewModel relationship)
    {
        var command = new UpdateResourceRelationshipNetworkCommand(resourceRelationshipNetworkId, relationship.Description, relationship.RelationshipType, relationship.RelationshipForm);
        return await RequestAsync(command);
    }

    [HttpGet]
    public async Task<IEnumrable<ResourceRelationshipNetworkViewModel>> GetResourceRelationshipNetworks([FromQuery] ResourceRelationshipNetworksQuery query)
    {
        return await _mediator.Send(query);
    }

    [HttpDelete]
    [Route("{resourceRelationshipNetworkId}")]
    public async Task<IActionResult> DeleteResourceRelationshipNetwork([FromRoute] string resourceRelationshipNetworkId)
    {
        var command = new DeleteResourceRelationshipNetworkCommand(resourceRelationshipNetworkId);
        return await RequestAsync(command);
    }

    [HttpPost]
    [Route("{resourceRelationshipNetworkId}/Connections")]
    public async Task<IActionResult> CreateConnection([FromRoute] string resourceRelationshipNetworkId, [FromBody] CreateResourceNetworkConnectionViewModel connection)
    {
        var command = new CreateResourceNetworkConnectionCommand(resourceRelationshipNetworkId, connection.ConnectionId, connection.Description, connection.Properties, connection.FromResource, connection.ToResource);
        return await RequestAsync(command);
    }

    [HttpPut]
    [Route("{resourceRelationshipNetworkId}/Connections/{connectionId}")]
    public async Task<IActionResult> UpdateConnection([FromRoute] string resourceRelationshipNetworkId, [FromRoute] string connectionId, [FromBody] UpdateResourceNetworkConnectionViewModel connection)
    {
        var command = new UpdateResourceNetworkConnectionCommand(resourceRelationshipNetworkId, connectionId, connection.Description, connection.Properties, connection.FromResource, connection.ToResource);
        return await RequestAsync(command);
    }

    [HttpDelete]
    [Route("{resourceRelationshipNetworkId}/Connections/{connectionId}")]
    public async Task<IActionResult> DeleteConnection([FromRoute] string resourceRelationshipNetworkId, [FromRoute] string connectionId)
    {
        var command = new DeleteResourceNetworkConnectionCommand(resourceRelationshipNetworkId, connectionId);
        return await RequestAsync(command);
    }

    [HttpDelete]
    [Route("{resourceRelationshipNetworkId}/Connections")]
    public async Task<IActionResult> DeleteConnectionsByResourceId([FromRoute] string resourceRelationshipNetworkId, string resourceId)
    {
        var command = new DeleteConnectionsByResourceIdCommand(resourceRelationshipNetworkId, resourceId);
        return await RequestAsync(command);
    }

    [HttpGet]
    [Route("{resourceRelationshipNetworkId}/Connections")]
    public async Task<IEnumrable<ResourceNetworkConnectionViewModel>> GetResourceRelationshipNetworkConnections([FromQuery] ResourceRelationshipConnectionsQuery query, [FromRoute] string resourceRelationshipNetworkId)
    {
        query.NetworkId = resourceRelationshipNetworkId;
        return await _mediator.Send(query);
    }
}
