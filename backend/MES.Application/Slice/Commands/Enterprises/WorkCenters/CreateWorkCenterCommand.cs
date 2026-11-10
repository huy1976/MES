using MesMicroservice.Application.Commands.Enterprises.WorkUnits;
using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Application.Commands.Enterprises.WorkCenters;

public class CreateWorkCenterCommand : IRequest<bool>
{
    public string EnterpriseId { get; set; }
    public string SiteId { get; set; }
    public string AreaId { get; set; }
    public string WorkCenterId { get; set; }
    public string Name { get; set; }
    public EWorkCenterType WorkCenterType { get; set; }
    public List<CreateWorkUnitViewModel> WorkUnits { get; set; }
    public CreateWorkCenterCommand(string enterpriseId, string siteId, string areaId, string workCenterId, string name, EWorkCenterType workCenterType, List<CreateWorkUnitViewModel> workUnits)
    {
        EnterpriseId = enterpriseId;
        SiteId = siteId;
        AreaId = areaId;
        WorkCenterId = workCenterId;
        Name = name;
        WorkCenterType = workCenterType;
        WorkUnits = workUnits;
    }
}
