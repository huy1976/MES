namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks;

public class DeleteResourceRelationshipNetworkCommand : IRequest<bool>
{
    public string ResourceRelationshipNetworkId { get; set; }

    public DeleteResourceRelationshipNetworkCommand(string resourceRelationshipNetworkId)
    {
        ResourceRelationshipNetworkId = resourceRelationshipNetworkId;
    }
}
