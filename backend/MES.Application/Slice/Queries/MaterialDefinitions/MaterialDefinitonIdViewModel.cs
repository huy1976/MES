namespace MesMicroservice.Application.Queries.MaterialDefinitions;

public class MaterialDefinitonIdViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }

    public MaterialDefinitonIdViewModel(string id, string name)
    {
        Id = id;
        Name = name;
    }
}
