using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Application.Commands.Enterprises.WorkCenters;
[DataContract]
public class UpdateWorkCenterViewModel
{
    [DataMember]
    public string WorkCenterId { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public EWorkCenterType WorkCenterType { get; set; }

    public UpdateWorkCenterViewModel(string workCenterId, string name, EWorkCenterType workCenterType)
    {
        WorkCenterId = workCenterId;
        Name = name;
        WorkCenterType = workCenterType;
    }
}
