using MesMicroservice.Application.Queries.EquipmentCapabilities;

namespace MesMicroservice.Application.Queries.Equipments;

public class EquipmentViewModel
{
    public string EquipmentId { get; set; }
    public string Name { get; set; }
    public EEquipmentLevel EquipmentLevel { get; set; }
    public EStatus EquipmentStatus { get; set; }
    public string HierarchyModelName { get; set; }
    public List<PropertyViewModel> Properties { get; set; }
    public EquipmentClassViewModel EquipmentClass { get; set; }
    public string AbsolutePath { get; set; }
    public List<EquipmentCapabilityViewModel> EquipmentCapabilities { get; set; }
    public List<EquipmentWorkOrder> EquipmentWorkOrders { get; set; }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public EquipmentViewModel() { }

    public EquipmentViewModel(string equipmentId, string name, EEquipmentLevel equipmentLevel, EStatus equipmentStatus, string hierarchyModelName, List<PropertyViewModel> properties, EquipmentClassViewModel equipmentClass, string absolutePath, List<EquipmentCapabilityViewModel> equipmentCapabilities, List<EquipmentWorkOrder> equipmentWorkOrders)
    {
        EquipmentId = equipmentId;
        Name = name;
        EquipmentLevel = equipmentLevel;
        EquipmentStatus = equipmentStatus;
        HierarchyModelName = hierarchyModelName;
        Properties = properties;
        EquipmentClass = equipmentClass;
        AbsolutePath = absolutePath;
        EquipmentCapabilities = equipmentCapabilities;
        EquipmentWorkOrders = equipmentWorkOrders;
    }









#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.





}
