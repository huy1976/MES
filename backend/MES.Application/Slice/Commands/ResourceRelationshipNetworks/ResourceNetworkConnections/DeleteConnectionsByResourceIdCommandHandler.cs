namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks.ResourceNetworkConnections;

public class DeleteConnectionsByResourceIdCommandHandler : IRequestHandler<DeleteConnectionsByResourceIdCommand, bool>
{
    private readonly IResourceRelationshipNetworkRepository _relationshipRepository;
    private readonly IResourceRepository _resourceRepository;

    public DeleteConnectionsByResourceIdCommandHandler(IResourceRelationshipNetworkRepository relationshipRepository, IResourceRepository resourceRepository)
    {
        _relationshipRepository = relationshipRepository;
        _resourceRepository = resourceRepository;
    }

    public async Task<bool> Handle(DeleteConnectionsByResourceIdCommand request, CancellationToken cancellationToken)
    {
        var relationship = await _relationshipRepository.GetAsync(request.ResourceRelationshipNetworkId)
            ?? throw new ResourceNotFoundException(nameof(ResourceRelationshipNetwork), request.ResourceRelationshipNetworkId);

        relationship.RemoveConnectionsByResourceId(request.ResourceId);

        return await _relationshipRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
