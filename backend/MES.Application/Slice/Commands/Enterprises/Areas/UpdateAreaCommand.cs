namespace MesMicroservice.Application.Commands.Enterprises.Areas;

public class UpdateAreaCommand : IRequest<bool>
{
    public string EnterpriseId { get; set; }
    public string SiteId { get; set; }
    public string AreaId { get; set; }
    public string HierarchyModelId { get; set; }
    public string Name { get; set; }

    public UpdateAreaCommand(string enterpriseId, string siteId, string areaId, string hierarchyModelId, string name)
    {
        EnterpriseId = enterpriseId;
        SiteId = siteId;
        AreaId = areaId;
        HierarchyModelId = hierarchyModelId;
        Name = name;
    }
}
    
