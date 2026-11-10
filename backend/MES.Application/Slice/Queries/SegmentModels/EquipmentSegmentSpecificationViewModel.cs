namespace MesMicroservice.Application.Queries.SegmentModels;

public class EquipmentSegmentSpecificationViewModel
{
    public string EquipmentSegmentSpecificationId { get; set;}
    public string? Description { get; set; }
    public EEquipmentUse EquipmentUse { get; set; }
    public Value Quantity { get; private set; }
    public string ValueUnitOfMeasure { get; private set; }
    public string ProcessSegmentId { get; set; }
    public string EquipmentClassName { get; set; }
    public List<PropertyViewModel> Properties { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public EquipmentSegmentSpecificationViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public EquipmentSegmentSpecificationViewModel(string equipmentSegmentSpecificationId, string? description, EEquipmentUse equipmentUse, Value quantity, string valueUnitOfMeasure, string processSegmentId, string equipmentClassName, List<PropertyViewModel> properties)
    {
        EquipmentSegmentSpecificationId = equipmentSegmentSpecificationId;
        Description = description;
        EquipmentUse = equipmentUse;
        Quantity = quantity;
        ValueUnitOfMeasure = valueUnitOfMeasure;
        ProcessSegmentId = processSegmentId;
        EquipmentClassName = equipmentClassName;
        Properties = properties;
    }
}
