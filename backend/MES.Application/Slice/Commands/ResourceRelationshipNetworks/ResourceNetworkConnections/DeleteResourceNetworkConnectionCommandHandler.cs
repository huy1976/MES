namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks.ResourceNetworkConnections;

public class DeleteResourceNetworkConnectionCommandHandler : IRequestHandler<DeleteResourceNetworkConnectionCommand, bool>
{
    private readonly IResourceRelationshipNetworkRepository _relationshipRepository;

    public DeleteResourceNetworkConnectionCommandHandler(IResourceRelationshipNetworkRepository relationshipRepository)
    {
        _relationshipRepository = relationshipRepository;
    }

    public async Task<bool> Handle(DeleteResourceNetworkConnectionCommand request, CancellationToken cancellationToken)
    {
        var relationship = await _relationshipRepository.GetAsync(request.ResourceRelationshipNetworkId)
            ?? throw new ResourceNotFoundException(nameof(ResourceRelationshipNetwork), request.ResourceRelationshipNetworkId);

        relationship.RemoveConnection(request.ConnectionId);

        return await _relationshipRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
