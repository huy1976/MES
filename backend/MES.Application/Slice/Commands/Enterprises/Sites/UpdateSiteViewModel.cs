namespace MesMicroservice.Application.Commands.Enterprises.Sites;
[DataContract]
public class UpdateSiteViewModel
{
    [DataMember]
    public string SiteId { get; set; }
    [DataMember]
    public string Name { get; set; }

    public UpdateSiteViewModel(string siteId, string name)
    {
        SiteId = siteId;
        Name = name;
    }
}
