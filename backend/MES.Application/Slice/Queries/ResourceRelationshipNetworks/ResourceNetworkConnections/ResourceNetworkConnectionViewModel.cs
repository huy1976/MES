namespace MesMicroservice.Application.Queries.ResourceRelationshipNetworks.ResourceNetworkConnections;

public class ResourceNetworkConnectionViewModel
{
    public string ConnectionId { get; set; }
    public string Description { get; set; }
    public List<PropertyViewModel> Properties { get; set; }
    public ConnectedResourceViewModel FromResource { get; set; }
    public ConnectedResourceViewModel ToResource { get; set; }

    public ResourceNetworkConnectionViewModel(string connectionId, string description, List<PropertyViewModel> properties, ConnectedResourceViewModel fromResource, ConnectedResourceViewModel toResource)
    {
        ConnectionId = connectionId;
        Description = description;
        Properties = properties;
        FromResource = fromResource;
        ToResource = toResource;
    }
}
