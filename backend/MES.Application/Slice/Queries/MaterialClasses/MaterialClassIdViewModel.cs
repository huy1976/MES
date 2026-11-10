namespace MesMicroservice.Application.Queries.MaterialClasses;

public class MaterialClassIdViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public MaterialClassIdViewModel(string id, string name)
    {
        Id = id;
        Name = name;
    }

}
