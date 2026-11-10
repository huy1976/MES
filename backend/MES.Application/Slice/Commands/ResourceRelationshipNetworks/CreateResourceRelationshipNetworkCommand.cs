namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks;

[DataContract]
public class CreateResourceRelationshipNetworkCommand : IRequest<bool>
{
    [DataMember]
    public string ResourceRelationshipNetworkId { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public ERelationshipType RelationshipType { get; set; }
    [DataMember]
    public ERelationshipForm RelationshipForm { get; set; }

    public CreateResourceRelationshipNetworkCommand(string resourceRelationshipNetworkId, string description, ERelationshipType relationshipType, ERelationshipForm relationshipForm)
    {
        ResourceRelationshipNetworkId = resourceRelationshipNetworkId;
        Description = description;
        RelationshipType = relationshipType;
        RelationshipForm = relationshipForm;
    }
}
