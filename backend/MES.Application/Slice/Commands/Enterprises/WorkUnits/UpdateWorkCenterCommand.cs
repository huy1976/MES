using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Application.Commands.Enterprises.WorkUnits;

public class UpdateWorkUnitCommand : IRequest<bool>
{
    public string EnterpriseId { get; set; }
    public string SiteId { get; set; }
    public string AreaId { get; set; }
    public string WorkCenterId { get; set; }
    public string WorkUnitId { get; set; }
    public string HierarchyModelId { get; set; }
    public string Name { get; set; }

    public UpdateWorkUnitCommand(string enterpriseId, string siteId, string areaId, string workCenterId, string workUnitId, string hierarchyModelId, string name)
    {
        EnterpriseId = enterpriseId;
        SiteId = siteId;
        AreaId = areaId;
        WorkCenterId = workCenterId;
        WorkUnitId = workUnitId;
        HierarchyModelId = hierarchyModelId;
        Name = name;
    }
}
