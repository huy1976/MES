namespace MesMicroservice.Application.Queries.EquipmentCapabilities;

public class EquipmentCapabilityViewModel
{
    public string EquipmentCapabilityId { get; set; }
    public string EquipmentName { get; set; }
    public string Description { get; set; }
    public string HierarchyScope { get; set; }
    public ECapabilityType CapabilityType { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public Value Quantity { get; set; }
    public string ValueUnitOfMeasure { get; set; }
    public List<PropertyViewModel> Properties { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public EquipmentCapabilityViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public EquipmentCapabilityViewModel(string equipmentCapabilityId, string equipmentName, string description, string hierarchyScope, ECapabilityType capabilityType, DateTime validFrom, DateTime validTo, Value quantity, string valueUnitOfMeasure, List<PropertyViewModel> properties)
    {
        EquipmentCapabilityId = equipmentCapabilityId;
        EquipmentName = equipmentName;
        Description = description;
        HierarchyScope = hierarchyScope;
        CapabilityType = capabilityType;
        ValidFrom = validFrom;
        ValidTo = validTo;
        Quantity = quantity;
        ValueUnitOfMeasure = valueUnitOfMeasure;
        Properties = properties;
    }
}
