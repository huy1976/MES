using MesMicroservice.Domain.AggregateModels.ResourceRelationshipNetworkAggregate;

namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks;

[DataContract]
public class UpdateResourceRelationshipNetworkViewModel
{
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public ERelationshipType RelationshipType { get; set; }
    [DataMember]
    public ERelationshipForm RelationshipForm { get; set; }

    public UpdateResourceRelationshipNetworkViewModel(string description, ERelationshipType relationshipType, ERelationshipForm relationshipForm)
    {
        Description = description;
        RelationshipType = relationshipType;
        RelationshipForm = relationshipForm;
    }
}
