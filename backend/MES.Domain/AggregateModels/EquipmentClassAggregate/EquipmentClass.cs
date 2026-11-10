namespace MesMicroservice.Domain.AggregateModels.EquipmentClassAggregate;
public class EquipmentClass : Resource, IAggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public EEquipmentLevel EquipmentLevel { get; private set; }
    public ICollection<Equipment> Equipments { get; private set; } = new List<Equipment>();
    public ICollection<EquipmentClassProperty> Properties { get; private set; } = new List<EquipmentClassProperty>();
    public ICollection<EquipmentSegmentSpecification> EquipmentSegmentSpecifications { get; set; } = new List<EquipmentSegmentSpecification>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private EquipmentClass() : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public EquipmentClass(string classId, string name, string description, EEquipmentLevel equipmentLevel, ICollection<EquipmentClassProperty> properties) : base(classId)
    {
        Name = name;
        Description = description;
        EquipmentLevel = equipmentLevel;
        Properties = properties;
    }

    public void Update(string name)
    {
        Name = name;
    }
    
}
