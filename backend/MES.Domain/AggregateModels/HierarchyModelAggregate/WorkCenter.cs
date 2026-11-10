using MesMicroservice.Domain.AggregateModels.WorkCenterOutputAggregate;

namespace MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;
public class WorkCenter : HierarchyModel
{
    public EWorkCenterType WorkCenterType { get; private set; }
    public List<WorkUnit> WorkUnits { get; private set; }
    public List<WorkCenterOutput> Outputs { get; private set; }
    public ICollection<WorkOrder> WorkOrders { get; private set; } = new List<WorkOrder>();

    public WorkCenter() : base()
    {
        WorkUnits = new List<WorkUnit>();
        Outputs = new List<WorkCenterOutput>();
    }
    public WorkCenter(string workCenterId, string name, EWorkCenterType workCenterType) : base(workCenterId, name)
    {
        WorkCenterType = workCenterType;
        WorkUnits = new List<WorkUnit>();
        Outputs = new List<WorkCenterOutput>();
    }

    public void Update(string workCenterId, string name, EWorkCenterType workCenterType)
    {
        HierarchyModelId = workCenterId;
        Name = name;
        WorkCenterType = workCenterType;
    }

    public void AddWorkUnit(WorkUnit workUnit)
    {
        if (WorkUnits.Any(x => x.HierarchyModelId == workUnit.HierarchyModelId))
        {
            throw new ChildEntityDuplicationException(workUnit.HierarchyModelId, workUnit, this.HierarchyModelId, this);
        }

        WorkUnits.Add(workUnit);
    }

    public void RemoveWorkUnit(string workUnitId)
    {
        var workUnit = WorkUnits.Find(x => x.HierarchyModelId == workUnitId)
            ?? throw new ChildEntityNotFoundException(workUnitId, typeof(WorkUnit), this.HierarchyModelId, this);

        WorkUnits.Remove(workUnit);
    }

    public void UpdateWorkUnit(string workUnitId, string name)
    {
        var workUnit = WorkUnits.Find(x => x.HierarchyModelId == workUnitId);
        workUnit?.Update(workUnitId, name);
    }
}
