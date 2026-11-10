using MesMicroservice.Application.Commands.Enterprises.Areas;

namespace MesMicroservice.Application.Commands.Enterprises.Sites;

[DataContract]
public class CreateSiteViewModel
{
    [DataMember]
    public string SiteId { get; set; }
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public List<CreateAreaViewModel> Areas { get; set; }
    public CreateSiteViewModel(string siteId, string name, List<CreateAreaViewModel> areas)
    {
        SiteId = siteId;
        Name = name;
        Areas = areas;
    }
}
