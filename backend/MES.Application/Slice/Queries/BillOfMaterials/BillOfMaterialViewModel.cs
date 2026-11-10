namespace MesMicroservice.Application.Queries.BillOfMaterials;

public class BillOfMaterialViewModel
{
    public string BillOfMaterialId { get; set; } 
    public string? Description { get; set; }
    public EMaterialUse MaterialUse { get; set; }
    public EAssemblyType AssemblyType { get; set; }
    public EAssemblyRelationship AssemblyRelationship { get; set; }
    public Value Quantity { get;  set; }
    public string ValueUnitOfMeasure { get;  set; }
    public string MaterialDefinition { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public BillOfMaterialViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public BillOfMaterialViewModel(string billOfMaterialId, string? description, EMaterialUse materialUse, EAssemblyType assemblyType, EAssemblyRelationship assemblyRelationship, Value quantity, string valueUnitOfMeasure, string materialDefinition)
    {
        BillOfMaterialId = billOfMaterialId;
        Description = description;
        MaterialUse = materialUse;
        AssemblyType = assemblyType;
        AssemblyRelationship = assemblyRelationship;
        Quantity = quantity;
        ValueUnitOfMeasure = valueUnitOfMeasure;
        MaterialDefinition = materialDefinition;
    }
}
