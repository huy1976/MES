namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks.ResourceNetworkConnections;

public class UpdateResourceNetworkConnectionCommandHandler : IRequestHandler<UpdateResourceNetworkConnectionCommand, bool>
{
    private readonly IResourceRelationshipNetworkRepository _relationshipRepository;
    private readonly IResourceRepository _resourceRepository;

    public UpdateResourceNetworkConnectionCommandHandler(IResourceRelationshipNetworkRepository relationshipRepository, IResourceRepository resourceRepository)
    {
        _relationshipRepository = relationshipRepository;
        _resourceRepository = resourceRepository;
    }

    public async Task<bool> Handle(UpdateResourceNetworkConnectionCommand request, CancellationToken cancellationToken)
    {
        var relationship = await _relationshipRepository.GetAsync(request.ResourceRelationshipNetworkId)
            ?? throw new ResourceNotFoundException(nameof(ResourceRelationshipNetwork), request.ResourceRelationshipNetworkId);

        var properties = request.Properties.ConvertAll(x => new ResourceNetworkConnectionProperty(
            x.PropertyId,
            x.Description,
            new Value(x.ValueString, x.ValueType),
            x.ValueUnitOfMeasure,
            x.Name))
;

        var fromResource = await _resourceRepository.GetAsync(request.FromResource) ?? throw new ResourceNotFoundException(nameof(Resource), request.FromResource);
        var toResource = await _resourceRepository.GetAsync(request.ToResource) ?? throw new ResourceNotFoundException(nameof(Resource), request.ToResource);

        relationship.UpdateResourceNetworkConnection(request.ConnectionId, request.Description, properties, fromResource, toResource);

        return await _relationshipRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
