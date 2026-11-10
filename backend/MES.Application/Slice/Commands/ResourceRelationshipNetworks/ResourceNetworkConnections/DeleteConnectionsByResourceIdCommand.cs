namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks.ResourceNetworkConnections;

public class DeleteConnectionsByResourceIdCommand : IRequest<bool>
{
    public string ResourceRelationshipNetworkId { get; set; }
    public string ResourceId { get; set; }

    public DeleteConnectionsByResourceIdCommand(string resourceRelationshipNetworkId, string resourceId)
    {
        ResourceRelationshipNetworkId = resourceRelationshipNetworkId;
        ResourceId = resourceId;
    }
}
