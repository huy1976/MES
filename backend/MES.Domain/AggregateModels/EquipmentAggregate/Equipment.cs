namespace MesMicroservice.Domain.AggregateModels.EquipmentAggregate;
public class Equipment : Resource, IAggregateRoot
{
    public string Name { get; private set; }
    public EEquipmentLevel EquipmentLevel { get; set;} 
    public EStatus EquipmentStatus { get; set;}
    public List<EquipmentProperty> Properties { get; private set; }
    public EquipmentClass EquipmentClass { get; private set; }
    public HierarchyModel? HierarchyModel { get; private set; }
    public ICollection<EquipmentCapability> EquipmentCapabilities { get; set; } = new List<EquipmentCapability>();

    public ICollection<EquipmentWorkOrder> EquipmentWorkOrders { get; set; } = new List<EquipmentWorkOrder>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Equipment(): base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Equipment(string equipmentId, string name, EEquipmentLevel equipmentLevel, EStatus equipmentStatus,List<EquipmentProperty> properties, EquipmentClass equipmentClass, 
                    HierarchyModel? hierarchyModel) : base(equipmentId)
    {
        Name = name;
        EquipmentLevel = equipmentLevel;
        Properties = properties;
        EquipmentClass = equipmentClass;
        HierarchyModel = hierarchyModel;
        EquipmentStatus = equipmentStatus;
    }

    public void AddProperties(List<EquipmentProperty> equipmentProperties)
    {
        foreach (var property in equipmentProperties)
        {
            Properties.Add(property);
        }
    }

    public void Update(string name, List<EquipmentProperty> properties, EquipmentClass equipmentClass, HierarchyModel? hierarchyModel)
    {
        Name = name;
        Properties = properties;
        EquipmentClass = equipmentClass;
        HierarchyModel = hierarchyModel;
    }

    public void UpdateStatus(EStatus status)
    {
        EquipmentStatus = status;
    }
}
