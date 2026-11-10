namespace MesMicroservice.Application.Queries.Equipments;

public class EquipmentIdViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }

    public EquipmentIdViewModel(string id, string name)
    {
        Id = id;
        Name = name;
    }
}
