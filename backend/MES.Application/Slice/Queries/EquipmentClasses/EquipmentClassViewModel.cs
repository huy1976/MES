namespace MesMicroservice.Application.Queries.EquipmentClasses;

public class EquipmentClassViewModel
{
    public string EquipmentClassId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Equipments { get; set; }
    public List<PropertyViewModel> Properties { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public EquipmentClassViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public EquipmentClassViewModel(string name)
    {
        Name = name;
    }


    public EquipmentClassViewModel(string equipmentClassId, string name, string description, List<string> equipments, List<PropertyViewModel> properties)
    {
        EquipmentClassId = equipmentClassId;
        Name = name;
        Description = description;
        Equipments = equipments;
        Properties = properties;
    }
}
