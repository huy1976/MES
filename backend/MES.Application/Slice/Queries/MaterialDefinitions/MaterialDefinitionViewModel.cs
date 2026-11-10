

namespace MesMicroservice.Application.Queries.MaterialDefinitions;

public class MaterialDefinitionViewModel
{
    public string MaterialDefinitionId { get; set; }
    public string Name { get; set; }
    public string PrimaryUnit { get; set; }
    public string MaterialClassId {  get; set; }
    public string MaterialClassName { get; set; }
    public List<PropertyViewModel> Properties { get; set; }
    public List<MaterialUnitViewModel> SecondaryUnits { get; set; }
    public List<OperationViewModel> Operations { get; set; }
    public List<OperationsDefinitionViewModel> OperationsDefinitions { get; set; }
    public List<QualityStandardViewModel> QualityStandards { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public MaterialDefinitionViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }
    public MaterialDefinitionViewModel(string materialDefinitionId, string name, string primaryUnit, string materialClassId, string materialClassName, List<PropertyViewModel> properties, List<MaterialUnitViewModel> secondaryUnits, List<OperationViewModel> operations, List<OperationsDefinitionViewModel> operationsDefinitions, List<QualityStandardViewModel> qualityStandards)
    {
        MaterialDefinitionId = materialDefinitionId;
        Name = name;
        PrimaryUnit = primaryUnit;
        MaterialClassId = materialClassId;
        MaterialClassName = materialClassName;
        Properties = properties;
        SecondaryUnits = secondaryUnits;
        Operations = operations;
        OperationsDefinitions = operationsDefinitions;
        QualityStandards = qualityStandards;
    }
}
