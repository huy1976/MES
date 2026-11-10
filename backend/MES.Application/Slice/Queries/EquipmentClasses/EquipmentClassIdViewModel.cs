namespace MesMicroservice.Application.Queries.EquipmentClasses;

public class EquipmentClassIdViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public EquipmentClassIdViewModel(string id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

}
