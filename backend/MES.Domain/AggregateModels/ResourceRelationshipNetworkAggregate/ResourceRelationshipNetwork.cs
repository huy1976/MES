namespace MesMicroservice.Domain.AggregateModels.ResourceRelationshipNetworkAggregate;
public class ResourceRelationshipNetwork: Entity, IAggregateRoot
{
    public string ResourceRelationshipNetworkId { get; private set; }
    public string Description { get; private set; }
    public ERelationshipType RelationshipType { get; private set; }
    public ERelationshipForm RelationshipForm { get; private set; }
    public List<ResourceNetworkConnection> Connections { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ResourceRelationshipNetwork() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public ResourceRelationshipNetwork(string resourceRelationshipNetworkId, string description, ERelationshipType relationshipType, ERelationshipForm relationshipForm)
    {
        ResourceRelationshipNetworkId = resourceRelationshipNetworkId;
        Description = description;
        RelationshipType = relationshipType;
        RelationshipForm = relationshipForm;
        Connections = new List<ResourceNetworkConnection>();
    }

    public void Update(string description, ERelationshipType relationshipType, ERelationshipForm relationshipForm)
    {
        Description = description;
        RelationshipType = relationshipType;
        RelationshipForm = relationshipForm;
    }

    public ResourceNetworkConnection GetResourceNetworkConnection(string connectionId)
    {
        var connection = Connections.Find(d => d.ConnectionId == connectionId) ?? throw new ChildEntityNotFoundException(connectionId, typeof(ResourceNetworkConnection), ResourceRelationshipNetworkId, this);
        return connection;
    }

    public void AddResourceNetworkConnection(string connectionId, string description, List<ResourceNetworkConnectionProperty> properties, Resource fromResource, Resource toResource)
    {
        var connection = new ResourceNetworkConnection(connectionId, description, properties, fromResource, toResource);
        if (Connections.Exists(d => d.ConnectionId == connectionId))
        {
            throw new ChildEntityDuplicationException(connectionId, typeof(ResourceNetworkConnection), ResourceRelationshipNetworkId, this);
        }

        Connections.Add(connection);
    }

    public void UpdateResourceNetworkConnection(string connectionId, string description, List<ResourceNetworkConnectionProperty> properties, Resource fromResource, Resource toResource)
    {
        var connection = GetResourceNetworkConnection(connectionId);

        try
        {
            connection.Update(description, properties, fromResource, toResource);
        }
        catch (Exception ex)
        {
            throw new DomainException($"ResourceRelationshipNetwork with id {ResourceRelationshipNetworkId} throw an exception. See inner exception for details.", ex);
        }
    }

    public void RemoveConnection(string connectionId)
    {
        var connection = GetResourceNetworkConnection(connectionId);

        try
        {
            Connections.Remove(connection);
        }
        catch (Exception ex)
        {
            throw new DomainException($"ResourceRelationshipNetwork with id {ResourceRelationshipNetworkId} throw an exception. See inner exception for details.", ex);
        }
    }

    public void RemoveConnectionsByResourceId(string resourceId)
    {
        try
        {
            Connections.RemoveAll(d => d.FromResource.ResourceId == resourceId);
            Connections.RemoveAll(d => d.ToResource.ResourceId == resourceId);
        }
        catch (Exception ex)
        {
            throw new DomainException($"ResourceRelationshipNetwork with id {ResourceRelationshipNetworkId} throw an exception. See inner exception for details.", ex);
        }
    }
}
