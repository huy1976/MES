using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Application.Queries.Enterprises;

public class WorkCenterViewModel
{
    public string WorkCenterId { get; set; }
    public int MainEquipmentCount { get; set; }
    public string Name { get; set; }
    public string AbsolutePath { get; set; }
    public EWorkCenterType WorkCenterType { get; set; }
    public List<WorkUnitViewModel> WorkUnits { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public WorkCenterViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }
    public WorkCenterViewModel(string workCenterId, int mainEquipmentCount, string name, string absolutePath, EWorkCenterType workCenterType, List<WorkUnitViewModel> workUnits)
    {
        WorkCenterId = workCenterId;
        MainEquipmentCount = mainEquipmentCount;
        Name = name;
        AbsolutePath = absolutePath;
        WorkCenterType = workCenterType;
        WorkUnits = workUnits;
    }
}
