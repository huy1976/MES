namespace MesMicroservice.Application.Queries.MaterialClasses;

public class MaterialClassViewModel
{
    public string MaterialClassId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public EAssemblyType AssemblyType { get; set; }
    public EAssemblyRelationship AssemblyRelationship { get; set; }
    public List<string> MaterialDefinitions { get; set; }
    public List<PropertyViewModel> Properties { get; set; }
        
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public MaterialClassViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public MaterialClassViewModel(string name)
    {
        Name = name;
    }

    public MaterialClassViewModel(string materialClassId, string name, string description, EAssemblyType assemblyType, EAssemblyRelationship assemblyRelationship, List<string> materialDefinitions, List<PropertyViewModel> properties)
    {
        MaterialClassId = materialClassId;
        Name = name;
        Description = description;
        AssemblyType = assemblyType;
        AssemblyRelationship = assemblyRelationship;
        MaterialDefinitions = materialDefinitions;
        Properties = properties;
    }
}
