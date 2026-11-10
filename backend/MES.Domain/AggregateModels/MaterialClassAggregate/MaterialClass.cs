namespace MesMicroservice.Domain.AggregateModels.MaterialClassAggregate;

public class MaterialClass : Resource, IAggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public EAssemblyType AssemblyType { get; private set; }
    public EAssemblyRelationship AssemblyRelationship { get; private set; }
    public ICollection<MaterialDefinition> MaterialDefinitions { get; private set; } = new List<MaterialDefinition>();
    public ICollection<MaterialClassProperty> Properties { get; private set; } = new List<MaterialClassProperty>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private MaterialClass() : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public MaterialClass(string materialClassId, string name, string description, EAssemblyType assemblyType, EAssemblyRelationship assemblyRelationship, ICollection<MaterialClassProperty> properties) : base(materialClassId)
    {
        Name = name;
        Description = description;
        AssemblyType = assemblyType;
        AssemblyRelationship = assemblyRelationship;
        Properties = properties;
    }

    public void AddProperties(List<MaterialClassProperty> materialClassProperties)
    {
        foreach (var property in materialClassProperties)
        {
            Properties.Add(property);
        }
    }


    public void Update(string name)
    {
        Name = name;
    }
}
