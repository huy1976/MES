namespace MesMicroservice.Application.Queries.ResourceRelationshipNetworks.ResourceNetworkConnections;

public class ConnectedResourceViewModel
{
    public string ResourceId { get; set; }
    public string ResourceType { get; set; }

    public ConnectedResourceViewModel(Resource resource)
    {
        ResourceId = resource.ResourceId;
        ResourceType = resource.GetType().Name;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ConnectedResourceViewModel() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
