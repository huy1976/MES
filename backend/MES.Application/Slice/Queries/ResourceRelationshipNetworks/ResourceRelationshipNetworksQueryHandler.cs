using MesMicroservice.Domain.AggregateModels.ResourceRelationshipNetworkAggregate;

namespace MesMicroservice.Application.Queries.ResourceRelationshipNetworks;

public class ResourceRelationshipNetworksQueryHandler : IRequestHandler<ResourceRelationshipNetworksQuery, IEnumrable<ResourceRelationshipNetworkViewModel>>
{
    private readonly IResourceRelationshipNetworkRepository _resourceRelationshipNetworkRepository;
    private readonly IMapper _mapper;

    public ResourceRelationshipNetworksQueryHandler(IResourceRelationshipNetworkRepository resourceRelationshipNetworkRepository, IMapper mapper)
    {
        _resourceRelationshipNetworkRepository = resourceRelationshipNetworkRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<ResourceRelationshipNetworkViewModel>> Handle(ResourceRelationshipNetworksQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _resourceRelationshipNetworkRepository.GetResourceRelationshipNetworks();

        if (request.IdStartedWith is not null)
        {
            queryable = queryable.Where(x => x.ResourceRelationshipNetworkId.StartsWith(request.IdStartedWith));
        }

        if (request.Paginated)
        {
            queryable = queryable
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        int totalItems = queryable.Count();

        var relationships = queryable.ToList();

        var queryResult = new IEnumrable<ResourceRelationshipNetwork>(relationships, totalItems);

        return _mapper.Map<IEnumrable<ResourceRelationshipNetwork>, IEnumrable<ResourceRelationshipNetworkViewModel>>(queryResult);
    }
}
