namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks.ResourceNetworkConnections;

public class DeleteResourceNetworkConnectionCommand : IRequest<bool>
{
    public string ResourceRelationshipNetworkId { get; set; }
    public string ConnectionId { get; set; }

    public DeleteResourceNetworkConnectionCommand(string resourceRelationshipNetworkId, string connectionId)
    {
        ResourceRelationshipNetworkId = resourceRelationshipNetworkId;
        ConnectionId = connectionId;
    }
}
