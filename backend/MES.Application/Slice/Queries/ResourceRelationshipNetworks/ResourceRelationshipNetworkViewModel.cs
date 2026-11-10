namespace MesMicroservice.Application.Queries.ResourceRelationshipNetworks;

public class ResourceRelationshipNetworkViewModel
{
    public string ResourceRelationshipNetworkId { get; set; }
    public string Description { get; set; }
    public ERelationshipType RelationshipType { get; set; }
    public ERelationshipForm RelationshipForm { get; set; }
    public List<ResourceNetworkConnectionViewModel> Connections { get; set; }

    public ResourceRelationshipNetworkViewModel(string resourceRelationshipNetworkId, string description, ERelationshipType relationshipType, ERelationshipForm relationshipForm, List<ResourceNetworkConnectionViewModel> connections)
    {
        ResourceRelationshipNetworkId = resourceRelationshipNetworkId;
        Description = description;
        RelationshipType = relationshipType;
        RelationshipForm = relationshipForm;
        Connections = connections;
    }
}
