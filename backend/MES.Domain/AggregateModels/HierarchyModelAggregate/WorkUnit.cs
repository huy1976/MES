using MesMicroservice.Domain.AggregateModels.EquipmentAggregate;

namespace MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;
public class WorkUnit : HierarchyModel
{
    public WorkUnit() : base()
    {
    }

    public WorkUnit(string workUnitId, string name) : base(workUnitId, name)
    {
    }

    public void Update(string workUnitId, string name)
    {
        HierarchyModelId = workUnitId;
        Name = name;
    }
}
