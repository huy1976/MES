namespace MesMicroservice.Application.Queries.SegmentModels;

public class MaterialSegmentSpecificationViewModel
{
    public string MaterialSegmentSpecificationId { get; set; }
    public string? Description { get; set; }
    public EAssemblyType AssemblyType { get; set; }
    public EAssemblyRelationship AssemblyRelationship { get; set; }
    public EMaterialUse MaterialUse { get; set; }
    public Value? Quantity { get; private set; }
    public string? ValueUnitOfMeasure { get; private set; }
    public string ProcessSegmentId { get; set; }
    public string MaterialDefinitionName { get; set; }
    public string MaterialClassName { get; set; }
    public List<PropertyViewModel> Properties { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public MaterialSegmentSpecificationViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public MaterialSegmentSpecificationViewModel(string materialSegmentSpecificationId, string? description, EAssemblyType assemblyType, EAssemblyRelationship assemblyRelationship, EMaterialUse materialUse, Value? quantity, string? valueUnitOfMeasure, string processSegmentId, string materialDefinitionName, string materialClassName, List<PropertyViewModel> properties)
    {
        MaterialSegmentSpecificationId = materialSegmentSpecificationId;
        Description = description;
        AssemblyType = assemblyType;
        AssemblyRelationship = assemblyRelationship;
        MaterialUse = materialUse;
        Quantity = quantity;
        ValueUnitOfMeasure = valueUnitOfMeasure;
        ProcessSegmentId = processSegmentId;
        MaterialDefinitionName = materialDefinitionName;
        MaterialClassName = materialClassName;
        Properties = properties;
    }
}
