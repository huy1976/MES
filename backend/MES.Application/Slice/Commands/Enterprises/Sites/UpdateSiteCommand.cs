namespace MesMicroservice.Application.Commands.Enterprises.Sites;

public class UpdateSiteCommand : IRequest<bool>
{
    public string EnterpriseId { get; set; }
    public string SiteId { get; set; }
    public string HierarchyModelId { get; set; }
    public string Name { get; set; }

    public UpdateSiteCommand(string enterpriseId, string siteId, string hierarchyModelId, string name)
    {
        EnterpriseId = enterpriseId;
        SiteId = siteId;
        HierarchyModelId = hierarchyModelId;
        Name = name;
    }
}
