namespace MesMicroservice.Application.Commands.Enterprises.Areas;

public class CreateAreaCommand : IRequest<bool>
{
    public string EnterpriseId { get; set; }
    public string SiteId { get; set; }
    public string AreaId { get; set; }
    public string Name { get; set; }

    public CreateAreaCommand(string enterpriseId, string siteId, string areaId, string name)
    {
        EnterpriseId = enterpriseId;
        SiteId = siteId;
        AreaId = areaId;
        Name = name;
    }
}
