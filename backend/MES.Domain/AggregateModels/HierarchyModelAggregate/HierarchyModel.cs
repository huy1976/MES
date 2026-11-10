using MesMicroservice.Domain.AggregateModels.EquipmentAggregate;

namespace MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;
public class HierarchyModel : Entity
{
    public string HierarchyModelId { get; protected set; }
    public string Name { get; protected set; }
    public HierarchyModel? Parent { get; protected set; }
    public string AbsolutePath
    {
        get
        {
            if (Parent is null)
            {
                return HierarchyModelId;
            }
            else
            {
                return Parent.AbsolutePath + "/" + HierarchyModelId;
            }
        }
    }

    public List<Equipment> AssociatedEquipments { get; private set; } = new List<Equipment>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public HierarchyModel() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public HierarchyModel(string hierarchyModelId, string name, HierarchyModel? parent = null)
    {
        HierarchyModelId = hierarchyModelId;
        Name = name;
        Parent = parent;
    }
}
