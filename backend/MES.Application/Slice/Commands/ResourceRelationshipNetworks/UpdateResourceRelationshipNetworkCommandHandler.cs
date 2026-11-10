namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks;

public class UpdateResourceRelationshipNetworkCommandHandler : IRequestHandler<UpdateResourceRelationshipNetworkCommand, bool>
{
    private readonly IResourceRelationshipNetworkRepository _relationshipRepository;

    public UpdateResourceRelationshipNetworkCommandHandler(IResourceRelationshipNetworkRepository relationshipRepository)
    {
        _relationshipRepository = relationshipRepository;
    }

    public async Task<bool> Handle(UpdateResourceRelationshipNetworkCommand request, CancellationToken cancellationToken)
    {
        var relationship = await _relationshipRepository.GetAsync(request.ResourceRelationshipNetworkId) 
            ?? throw new ResourceNotFoundException(nameof(ResourceRelationshipNetwork), request.ResourceRelationshipNetworkId);

        relationship.Update(request.Description, request.RelationshipType, request.RelationshipForm);

        return await _relationshipRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
