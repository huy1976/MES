namespace MesMicroservice.Application.Queries.SegmentModels;

public class PersonnelSegmentSpecificationViewModel
{
    public string PersonnelSegmentSpecificationId { get; set; }
    public string? Description { get; set; }
    public EPersonnelUse PersonnelUse { get; set; }
    public Value Quantity { get; private set; }
    public string ValueUnitOfMeasure { get; private set; }
    public string ProcessSegmentId { get; set; }
    public string PersonnelClassName { get; set; }
    public List<PropertyViewModel> Properties { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public PersonnelSegmentSpecificationViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }
    public PersonnelSegmentSpecificationViewModel(string personnelSegmentSpecificationId, string? description, EPersonnelUse personnelUse, Value quantity, string valueUnitOfMeasure, string processSegmentId, string personnelClassName, List<PropertyViewModel> properties)
    {
        PersonnelSegmentSpecificationId = personnelSegmentSpecificationId;
        Description = description;
        PersonnelUse = personnelUse;
        Quantity = quantity;
        ValueUnitOfMeasure = valueUnitOfMeasure;
        ProcessSegmentId = processSegmentId;
        PersonnelClassName = personnelClassName;
        Properties = properties;
    }
}
