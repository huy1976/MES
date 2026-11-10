namespace MesMicroservice.Application.Commands.Enterprises.Areas;

public class DeleteAreaCommand : IRequest<bool>
{
    public string EnterpriseId { get; set; }
    public string SiteId { get; set; }
    public string AreaId { get; set; }

    public DeleteAreaCommand(string enterpriseId, string siteId, string areaId)
    {
        EnterpriseId = enterpriseId;
        SiteId = siteId;
        AreaId = areaId;
    }
}
