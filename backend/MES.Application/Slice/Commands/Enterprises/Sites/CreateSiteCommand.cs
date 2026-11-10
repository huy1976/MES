using MesMicroservice.Application.Commands.Enterprises.Areas;

namespace MesMicroservice.Application.Commands.Enterprises.Sites;

public class CreateSiteCommand : IRequest<bool>
{
    public string EnterpriseId { get; set; }
    public string SiteId { get; set; }
    public string Name { get; set; }
    public List<CreateAreaViewModel> Areas { get; set; }
    public CreateSiteCommand(string enterpriseId, string siteId, string name, List<CreateAreaViewModel> areas)
    {
        EnterpriseId = enterpriseId;
        SiteId = siteId;
        Name = name;
        Areas = areas;
    }
}
