namespace MesMicroservice.Application.Queries.ResourceRelationshipNetworks;

public class ResourceRelationshipNetworksQuery : PaginatedQuery, IRequest<IEnumrable<ResourceRelationshipNetworkViewModel>>
{
    public string? IdStartedWith { get; set; }
}
