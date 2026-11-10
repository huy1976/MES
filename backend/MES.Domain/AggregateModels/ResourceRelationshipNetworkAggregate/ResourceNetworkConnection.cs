namespace MesMicroservice.Domain.AggregateModels.ResourceRelationshipNetworkAggregate;
public class ResourceNetworkConnection : Entity
{
    public string ConnectionId { get; private set; }
    public string Description { get; private set; }
    public List<ResourceNetworkConnectionProperty> Properties { get; private set; }
    public Resource FromResource { get; private set; }
    public Resource ToResource { get; private set; }
    public int FromResourceId { get; private set; }
    public int ToResourceId { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ResourceNetworkConnection() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public ResourceNetworkConnection(string connectionId, string description, List<ResourceNetworkConnectionProperty> properties, Resource fromResource, Resource toResource)
    {
        ConnectionId = connectionId;
        Description = description;
        Properties = properties;
        FromResource = fromResource;
        ToResource = toResource;
        FromResourceId = FromResource.Id;
        ToResourceId = ToResource.Id;
    }

    public void Update(string description, List<ResourceNetworkConnectionProperty> properties, Resource fromResource, Resource toResource)
    {
        Description = description;
        Properties = properties;
        FromResource = fromResource;
        ToResource = toResource;
    }
}
