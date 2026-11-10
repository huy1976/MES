namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks.ResourceNetworkConnections;

public class CreateResourceNetworkConnectionCommandHandler : IRequestHandler<CreateResourceNetworkConnectionCommand, bool>
{
    private readonly IResourceRelationshipNetworkRepository _relationshipRepository;
    private readonly IResourceRepository _resourceRepository;

    public CreateResourceNetworkConnectionCommandHandler(IResourceRelationshipNetworkRepository relationshipRepository, IResourceRepository resourceRepository)
    {
        _relationshipRepository = relationshipRepository;
        _resourceRepository = resourceRepository;
    }

    public async Task<bool> Handle(CreateResourceNetworkConnectionCommand request, CancellationToken cancellationToken)
    {
        var relationship = await _relationshipRepository.GetAsync(request.ResourceRelationshipNetworkId)
            ?? throw new ResourceNotFoundException(nameof(ResourceRelationshipNetwork), request.ResourceRelationshipNetworkId);

        var properties = request.Properties.Select(x => new ResourceNetworkConnectionProperty(
            x.PropertyId,
            x.Description,
            new Value(x.ValueString, x.ValueType),
            x.ValueUnitOfMeasure,
            x.Name))
            .ToList();

        var fromResource = await _resourceRepository.GetAsync(request.FromResource) ?? throw new ResourceNotFoundException(nameof(Resource), request.FromResource);
        var toResource = await _resourceRepository.GetAsync(request.ToResource) ?? throw new ResourceNotFoundException(nameof(Resource), request.ToResource);

        relationship.AddResourceNetworkConnection(request.ConnectionId, request.Description, properties, fromResource, toResource);

        return await _relationshipRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
