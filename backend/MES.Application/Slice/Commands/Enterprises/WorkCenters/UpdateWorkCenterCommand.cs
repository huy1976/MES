using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Application.Commands.Enterprises.WorkCenters;

public class UpdateWorkCenterCommand : IRequest<bool>
{
    public string EnterpriseId { get; set; }
    public string SiteId { get; set; }
    public string AreaId { get; set; }
    public string WorkCenterId { get; set; }
    public string HierarchyModelId { get; set; }
    public string Name { get; set; }
    public EWorkCenterType WorkCenterType { get; set; }

    public UpdateWorkCenterCommand(string enterpriseId, string siteId, string areaId, string workCenterId, string hierarchyModelId, string name, EWorkCenterType workCenterType)
    {
        EnterpriseId = enterpriseId;
        SiteId = siteId;
        AreaId = areaId;
        WorkCenterId = workCenterId;
        HierarchyModelId = hierarchyModelId;
        Name = name;
        WorkCenterType = workCenterType;
    }
}
