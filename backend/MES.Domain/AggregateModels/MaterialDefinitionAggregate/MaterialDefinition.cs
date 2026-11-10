global using MesMicroservice.Domain.AggregateModels.BomAggregate;

namespace MesMicroservice.Domain.AggregateModels.MaterialDefinitionAggregate;
public class MaterialDefinition : Resource, IAggregateRoot
{
    public string Name { get; private set; }
    public string PrimaryUnit { get; private set; }
    public EAssemblyType AssemblyType { get; private set; }
    public EAssemblyRelationship AssemblyRelationship { get; private set; }
    public ICollection<MaterialDefinitionProperty> Properties { get; private set; } = new List<MaterialDefinitionProperty>();
    public MaterialClass MaterialClass { get; private set; }
    public ICollection<MaterialLot> MaterialLots { get; private set; } = new List<MaterialLot>();
    public ICollection<MaterialSubLot> MaterialSubLots { get; private set; } = new List<MaterialSubLot>();
    public ICollection<MaterialUnit> SecondaryUnits { get; private set; } = new List<MaterialUnit>();
    public ICollection<QualityStandard> QualityStandards { get; set; } = new List<QualityStandard>();
    public ICollection<MaterialSegmentSpecification> MaterialSegmentSpecifications { get; set; } = new List<MaterialSegmentSpecification>();
    public ICollection<BillOfMaterial> BillOfMaterials { get; set; } = new List<BillOfMaterial>();
    public ICollection<ManufacturingOrder> ManufacturingOrders { get; set; } = new List<ManufacturingOrder>();
    public ICollection<MaterialWorkOrder> MaterialWorkOrders { get; set; } = new List<MaterialWorkOrder>();
    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    public ICollection<OperationsDefinition> OperationsDefinitions { get; private set; } = new List<OperationsDefinition>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private MaterialDefinition() : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public MaterialDefinition(string materialDefinitionId, string name, string primaryUnit, EAssemblyType assemblyType, EAssemblyRelationship assemblyRelationship, List<MaterialDefinitionProperty> properties, MaterialClass materialClass): base(materialDefinitionId)
    {
        Name = name;
        PrimaryUnit = primaryUnit;
        AssemblyType = assemblyType;
        AssemblyRelationship = assemblyRelationship;
        Properties = properties;
        MaterialClass = materialClass;
    }

    public void AddProperties(List<MaterialDefinitionProperty> materialDefinitionProperties)
    {
        foreach (var property in materialDefinitionProperties)
        {
            Properties.Add(property);
        }
    }

    public void Update(string name, string primaryUnit, List<MaterialDefinitionProperty> properties, MaterialClass materialClass)
    {
        Name = name;
        PrimaryUnit = primaryUnit;
        Properties = properties;
        MaterialClass = materialClass;
    }

    public void AddMaterialUnit(string unitId, string unitName, decimal conversionValueToPrimaryUnit)
    {
        var materialUnit = new MaterialUnit(unitId, unitName, conversionValueToPrimaryUnit, Id);
        if(SecondaryUnits.Any(d => d.UnitId == unitId))
        {
            throw new ChildEntityDuplicationException(unitId, materialUnit, ResourceId, this);
        }

        SecondaryUnits.Add(materialUnit);
    }

    public MaterialUnit GetMaterialUnitWithId(string unitId)
    {
        var materialUnit = SecondaryUnits.FirstOrDefault(d => d.UnitId == unitId) ?? throw new ChildEntityNotFoundException(unitId, typeof(MaterialUnit), ResourceId, this);
        return materialUnit;
    }

    public void UpdateMaterialUnit(string unitId, string unitName, decimal conversionValueToPrimaryUnit)
    {
        var materialUnit = GetMaterialUnitWithId(unitId);

        try
        {
            materialUnit.Update(unitName, conversionValueToPrimaryUnit);
        }
        catch (Exception ex)
        {
            throw new DomainException($"MaterialDefinition with id {ResourceId} throw an exception. See inner exception for details.", ex);
        }
    }

    public void RemoveMaterialUnit(string unitId)
    {
        var materialUnit = GetMaterialUnitWithId(unitId);

        try
        {
            SecondaryUnits.Remove(materialUnit);
        }
        catch (Exception ex)
        {
            throw new DomainException($"MaterialDefinition with id {ResourceId} throw an exception. See inner exception for details.", ex);
        }
    }

    public void AddOperationsDefinition(string operationId, string name, List<string> prerequisiteOperationsDefinitionId)
    {
        //List<OperationsDefinition> prerequisiteOperationsDefinitions = prerequisiteOperationsDefinitionId.ConvertAll(id => GetOperationsDefinitionWithId(id));

        //var operation = new OperationsDefinition(operationId, name, prerequisiteOperationsDefinitions, Id);

        //if (OperationsDefinitions.Any(d => d.OperationsDefinitionId == operationId))
        //{
        //    throw new ChildEntityDuplicationException(operationId, operation, ResourceId, this);
        //}

        //OperationsDefinitions.Add(operation);
    }

    public OperationsDefinition GetOperationsDefinitionWithId(string operationId)
    {
        var operation = OperationsDefinitions.FirstOrDefault(d => d.ResourceId == operationId) ?? throw new ChildEntityNotFoundException(operationId, typeof(OperationsDefinition), ResourceId, this);
        return operation;
    }

    public void UpdateOperationsDefinition(string operationId, string name, List<string> prerequisiteOperationsDefinitionId)
    {
        //List<OperationsDefinition> prerequisiteOperationsDefinitions = new();

        //if (prerequisiteOperationsDefinitionId.Count > 0)
        //{
        //    prerequisiteOperationsDefinitions = prerequisiteOperationsDefinitionId.ConvertAll(id => GetOperationsDefinitionWithId(id));
        //}

        //var operation = GetOperationsDefinitionWithId(operationId);

        //try
        //{
        //    operation.Update(name, prerequisiteOperationsDefinitions);
        //}
        //catch (Exception ex)
        //{
        //    throw new DomainException($"MaterialDefinition with id {ResourceId} throw an exception. See inner exception for details.", ex);
        //}
    }

    public void RemoveOperationsDefinition(string operationId)
    {
        var operation = GetOperationsDefinitionWithId(operationId);

        try
        {
            OperationsDefinitions.Remove(operation);
        }
        catch (Exception ex)
        {
            throw new DomainException($"MaterialDefinition with id {ResourceId} throw an exception. See inner exception for details.", ex);
        }
    }
}
