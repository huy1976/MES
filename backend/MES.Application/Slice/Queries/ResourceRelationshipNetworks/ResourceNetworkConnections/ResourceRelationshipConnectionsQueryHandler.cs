using MesMicroservice.Domain.AggregateModels.ResourceRelationshipNetworkAggregate;

namespace MesMicroservice.Application.Queries.ResourceRelationshipNetworks.ResourceNetworkConnections;

public class ResourceRelationshipConnectionsQueryHandler : IRequestHandler<ResourceRelationshipConnectionsQuery, IEnumrable<ResourceNetworkConnectionViewModel>>
{
    private readonly IResourceRelationshipNetworkRepository _resourceRelationshipNetworkRepository;
    private readonly IMapper _mapper;

    public ResourceRelationshipConnectionsQueryHandler(IResourceRelationshipNetworkRepository resourceRelationshipNetworkRepository, IMapper mapper)
    {
        _resourceRelationshipNetworkRepository = resourceRelationshipNetworkRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<ResourceNetworkConnectionViewModel>> Handle(ResourceRelationshipConnectionsQuery request, CancellationToken cancellationToken)
    {
        var networks = await _resourceRelationshipNetworkRepository.GetResourceRelationshipNetworksById(request.NetworkId);

        var queryable = networks
            .SelectMany(x => x.Connections);

        if (request.IdStartedWith is not null)
        {
            queryable = queryable.Where(x => x.ConnectionId.StartsWith(request.IdStartedWith));
        }

        if (request.FromResourceId is not null)
        {
            queryable = queryable.Where(x => x.FromResource.ResourceId == request.FromResourceId);
        }

        if (request.ToResourceId is not null)
        {
            queryable = queryable.Where(x => x.ToResource.ResourceId == request.ToResourceId);
        }

        if (request.Paginated)
        {
            queryable = queryable
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        int totalItems = queryable.Count();

        var connections = queryable.ToList();

        var queryResult = new IEnumrable<ResourceNetworkConnection>(connections, totalItems);

        return _mapper.Map<IEnumrable<ResourceNetworkConnection>, IEnumrable<ResourceNetworkConnectionViewModel>>(queryResult);
    }
}
