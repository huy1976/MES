using MesMicroservice.Application.Commands.Enterprises.WorkUnits;
using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Application.Commands.Enterprises.WorkCenters;

[DataContract]
public class CreateWorkCenterViewModel
{
    [DataMember]
    public string WorkCenterId { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public EWorkCenterType WorkCenterType { get; set; }

    [DataMember]
    public List<CreateWorkUnitViewModel> WorkUnits { get; set; }

    public CreateWorkCenterViewModel(string workCenterId, string name, EWorkCenterType workCenterType, List<CreateWorkUnitViewModel> workUnits)
    {
        WorkCenterId = workCenterId;
        Name = name;
        WorkCenterType = workCenterType;
        WorkUnits = workUnits;
    }
}
