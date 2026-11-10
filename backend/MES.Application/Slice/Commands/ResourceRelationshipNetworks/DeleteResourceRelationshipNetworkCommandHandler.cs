using MesMicroservice.Domain.AggregateModels.ResourceRelationshipNetworkAggregate;

namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks;

public class DeleteResourceRelationshipNetworkCommandHandler : IRequestHandler<DeleteResourceRelationshipNetworkCommand, bool>
{
    private readonly IResourceRelationshipNetworkRepository _relationshipRepository;

    public DeleteResourceRelationshipNetworkCommandHandler(IResourceRelationshipNetworkRepository relationshipRepository)
    {
        _relationshipRepository = relationshipRepository;
    }

    public async Task<bool> Handle(DeleteResourceRelationshipNetworkCommand request, CancellationToken cancellationToken)
    {
        await _relationshipRepository.DeleteAsync(request.ResourceRelationshipNetworkId);

        return await _relationshipRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
