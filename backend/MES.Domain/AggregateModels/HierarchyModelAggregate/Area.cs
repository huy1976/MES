namespace MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;
public class Area: HierarchyModel
{
    public List<WorkCenter> WorkCenters { get; private set; }

    public Area() : base()
    {
        WorkCenters = new List<WorkCenter>();
    }

    public Area(string areaId, string name) : base(areaId, name)
    {
        WorkCenters = new List<WorkCenter>();
    }

    public void Update(string areaId, string name)
    {
        HierarchyModelId = areaId;
        Name = name;
    }

    public void AddWorkCenter(WorkCenter workCenter)
    {
        if (WorkCenters.Any(x => x.HierarchyModelId == workCenter.HierarchyModelId))
        {
            throw new ChildEntityDuplicationException(workCenter.HierarchyModelId, workCenter, this.HierarchyModelId, this);
        }

        WorkCenters.Add(workCenter);
    }

    public void RemoveWorkCenter(string workCenterId)
    {
        var workCenter = WorkCenters.Find(x => x.HierarchyModelId == workCenterId)
            ?? throw new ChildEntityNotFoundException(workCenterId, typeof(WorkCenter), this.HierarchyModelId, this);

        WorkCenters.Remove(workCenter);
    }
}
