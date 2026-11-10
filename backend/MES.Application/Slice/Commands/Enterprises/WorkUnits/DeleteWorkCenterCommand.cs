namespace MesMicroservice.Application.Commands.Enterprises.WorkUnits;

public class DeleteWorkUnitCommand : IRequest<bool>
{
    public string EnterpriseId { get; set; }
    public string SiteId { get; set; }
    public string AreaId { get; set; }
    public string WorkCenterId { get; set; }
    public string WorkUnitId { get; set; }

    public DeleteWorkUnitCommand(string enterpriseId, string siteId, string areaId, string workCenterId, string workUnitId)
    {
        EnterpriseId = enterpriseId;
        SiteId = siteId;
        AreaId = areaId;
        WorkCenterId = workCenterId;
        WorkUnitId = workUnitId;
    }
}
