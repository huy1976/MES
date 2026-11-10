namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks.ResourceNetworkConnections;

public class CreateResourceNetworkConnectionCommand : IRequest<bool>
{
    public string ResourceRelationshipNetworkId { get; set; }
    public string ConnectionId { get; set; }
    public string Description { get; set; }
    public List<SavePropertyViewModel> Properties { get; set; }
    public string FromResource { get; set; }
    public string ToResource { get; set; }

    public CreateResourceNetworkConnectionCommand(string resourceRelationshipNetworkId, string connectionId, string description, List<SavePropertyViewModel> properties, string fromResource, string toResource)
    {
        ResourceRelationshipNetworkId = resourceRelationshipNetworkId;
        ConnectionId = connectionId;
        Description = description;
        Properties = properties;
        FromResource = fromResource;
        ToResource = toResource;
    }
}
