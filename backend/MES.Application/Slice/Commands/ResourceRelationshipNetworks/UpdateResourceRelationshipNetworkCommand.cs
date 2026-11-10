using MesMicroservice.Domain.AggregateModels.ResourceRelationshipNetworkAggregate;

namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks;

public class UpdateResourceRelationshipNetworkCommand : IRequest<bool>
{
    public string ResourceRelationshipNetworkId { get; set; }
    public string Description { get; set; }
    public ERelationshipType RelationshipType { get; set; }
    public ERelationshipForm RelationshipForm { get; set; }

    public UpdateResourceRelationshipNetworkCommand(string resourceRelationshipNetworkId, string description, ERelationshipType relationshipType, ERelationshipForm relationshipForm)
    {
        ResourceRelationshipNetworkId = resourceRelationshipNetworkId;
        Description = description;
        RelationshipType = relationshipType;
        RelationshipForm = relationshipForm;
    }
}
