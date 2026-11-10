using MesMicroservice.Domain.AggregateModels.ResourceRelationshipNetworkAggregate;

namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks;

public class CreateResourceRelationshipNetworkCommandHandler : IRequestHandler<CreateResourceRelationshipNetworkCommand, bool>
{
    private readonly IResourceRelationshipNetworkRepository _relationshipRepository;

    public CreateResourceRelationshipNetworkCommandHandler(IResourceRelationshipNetworkRepository relationshipRepository)
    {
        _relationshipRepository = relationshipRepository;
    }

    public async Task<bool> Handle(CreateResourceRelationshipNetworkCommand request, CancellationToken cancellationToken)
    {
        var relationship = new ResourceRelationshipNetwork(request.ResourceRelationshipNetworkId, request.Description, request.RelationshipType, request.RelationshipForm);

        await _relationshipRepository.Add(relationship);

        return await _relationshipRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
