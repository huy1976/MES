namespace MesMicroservice.Application.Commands.Enterprises.WorkCenters;

public class DeleteWorkCenterCommand : IRequest<bool>
{
    public string EnterpriseId { get; set; }
    public string SiteId { get; set; }
    public string AreaId { get; set; }
    public string WorkCenterId { get; set; }

    public DeleteWorkCenterCommand(string enterpriseId, string siteId, string areaId, string workCenterId)
    {
        EnterpriseId = enterpriseId;
        SiteId = siteId;
        AreaId = areaId;
        WorkCenterId = workCenterId;
    }
}
