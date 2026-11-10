namespace MesMicroservice.Application.Commands.ResourceRelationshipNetworks.ResourceNetworkConnections;

[DataContract]
public class UpdateResourceNetworkConnectionViewModel
{
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public List<SavePropertyViewModel> Properties { get; set; }
    [DataMember]
    public string FromResource { get; set; }
    [DataMember]
    public string ToResource { get; set; }

    public UpdateResourceNetworkConnectionViewModel(string description, List<SavePropertyViewModel> properties, string fromResource, string toResource)
    {
        Description = description;
        Properties = properties;
        FromResource = fromResource;
        ToResource = toResource;
    }
}
