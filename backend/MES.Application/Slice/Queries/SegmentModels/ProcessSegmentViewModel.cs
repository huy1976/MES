namespace MesMicroservice.Application.Queries.SegmentModels;

public class ProcessSegmentViewModel
{
    public string ProcessSegmentId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    // [ĐÃ ĐỔI] Trả loại công đoạn dưới dạng dữ liệu động.
    public string SegmentType { get; set; }

    public EOperationsType OperationsType { get; set; }
    public string? HierarchyScope { get; set; }
    public Value Duration { get; set; }
    public string ValueUnitOfMeasure { get; set; }
    public string ProductionParameter { get; set; }
    public int EquipmentSegmentSpecificationCount { get; set; }
    public int MaterialSegmentSpecificationCount { get; set; }
    public int PersonnelSegmentSpecificationCount { get; set; }

#pragma warning disable CS8618
    public ProcessSegmentViewModel()
#pragma warning restore CS8618
    {
    }

    public ProcessSegmentViewModel(string processSegmentId)
    {
        ProcessSegmentId = processSegmentId;
    }

    public ProcessSegmentViewModel(
        string processSegmentId,
        string name,
        string? description,
        string segmentType,
        EOperationsType operationsType,
        string? hierarchyScope,
        Value duration,
        string valueUnitOfMeasure,
        string productionParameter,
        int equipmentSegmentSpecificationCount,
        int materialSegmentSpecificationCount,
        int personnelSegmentSpecificationCount) : this(processSegmentId)
    {
        Name = name;
        Description = description;
        SegmentType = segmentType;
        OperationsType = operationsType;
        HierarchyScope = hierarchyScope;
        Duration = duration;
        ValueUnitOfMeasure = valueUnitOfMeasure;
        ProductionParameter = productionParameter;
        EquipmentSegmentSpecificationCount = equipmentSegmentSpecificationCount;
        MaterialSegmentSpecificationCount = materialSegmentSpecificationCount;
        PersonnelSegmentSpecificationCount = personnelSegmentSpecificationCount;
    }
}
