namespace MesMicroservice.Application.Queries.ProductionParameters;

public class ProductionParameterViewModel
{
    public string ProductionParameterId { get; set; }
    public string? Description { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public string Status { get; set; }
    public List<PropertyViewModel> Properties { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ProductionParameterViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }
    public ProductionParameterViewModel(string productionParameterId, string? description, DateTime validFrom, DateTime validTo, string status, List<PropertyViewModel> properties)
    {
        ProductionParameterId = productionParameterId;
        Description = description;
        ValidFrom = validFrom;
        ValidTo = validTo;
        Status = status;
        Properties = properties;
    }
}
