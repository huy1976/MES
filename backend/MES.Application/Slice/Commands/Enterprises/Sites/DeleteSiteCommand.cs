namespace MesMicroservice.Application.Commands.Enterprises.Sites;

public class DeleteSiteCommand : IRequest<bool>
{
    public string EnterpriseId { get; set; }
    public string SiteId { get; set; }

    public DeleteSiteCommand(string enterpriseId, string siteId)
    {
        EnterpriseId = enterpriseId;
        SiteId = siteId;
    }
}
