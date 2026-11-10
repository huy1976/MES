namespace MesMicroservice.Application.Queries.ResourceRelationshipNetworks.ResourceNetworkConnections;

public class ResourceRelationshipConnectionsQuery: PaginatedQuery, IRequest<IEnumrable<ResourceNetworkConnectionViewModel>>
{
    public string NetworkId { get; set; } = "";
    public string? IdStartedWith { get; set; }
    public string? FromResourceId { get; set; }
    public string? ToResourceId { get; set;}
}
