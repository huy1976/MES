using MesMicroservice.Application.Queries.OperationsDefinitions;
using MesMicroservice.Application.Queries.OperationsSegments;

namespace MesMicroservice.Application.Queries.MaterialDefinitions;

public class OperationsDefinitionViewModel
{
    public string OperationId { get; set; }
    public string Version { get; set; }
    public string? Description { get; set; }
    public string OperationsType { get; set; }
    public string? HierarchyScope { get; set; }
    public string? WorkDefinitionId { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string MaterialDefinitionId { get; set; }
    public string? MaterialDefinitionName { get; set; }
    public List<BillOfMaterialViewModel> BillOfMaterials { get; set; }
    public List<OperationsSegmentViewModel> OperationsSegments { get; set; } 
    public List<OperationsSegmentDependencyViewModel> OperationsSegmentDependencies { get; set; }
    public string? OperationsMaterialBillId { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public OperationsDefinitionViewModel()
    {
    }
    public OperationsDefinitionViewModel(string operationId, string version, string? description, string operationsType, string? hierarchyScope, string? workDefinitionId, DateTime validFrom, DateTime? validTo, string materialDefinitionId, string? materialDefinitionName, List<BillOfMaterialViewModel> billOfMaterials, List<OperationsSegmentViewModel> operationsSegments, List<OperationsSegmentDependencyViewModel> operationsSegmentDependencies, string? operationsMaterialBillId)
    {
        OperationId = operationId;
        Version = version;
        Description = description;
        OperationsType = operationsType;
        HierarchyScope = hierarchyScope;
        WorkDefinitionId = workDefinitionId;
        ValidFrom = validFrom;
        ValidTo = validTo;
        MaterialDefinitionId = materialDefinitionId;
        MaterialDefinitionName = materialDefinitionName;
        BillOfMaterials = billOfMaterials;
        OperationsSegments = operationsSegments;
        OperationsSegmentDependencies = operationsSegmentDependencies;
        OperationsMaterialBillId = operationsMaterialBillId;
    }


#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.



}
