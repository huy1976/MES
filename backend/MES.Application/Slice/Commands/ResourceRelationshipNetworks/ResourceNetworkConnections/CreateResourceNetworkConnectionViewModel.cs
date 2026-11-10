namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks.ResourceNetworkConnections;

[DataContract]
public class CreateResourceNetworkConnectionViewModel
{
    [DataMember]
    public string ConnectionId { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public List<SavePropertyViewModel> Properties { get; set; }
    [DataMember]
    public string FromResource { get; set; }
    [DataMember]
    public string ToResource { get; set; }

    public CreateResourceNetworkConnectionViewModel(string connectionId, string description, List<SavePropertyViewModel> properties, string fromResource, string toResource)
    {
        ConnectionId = connectionId;
        Description = description;
        Properties = properties;
        FromResource = fromResource;
        ToResource = toResource;
    }
}
