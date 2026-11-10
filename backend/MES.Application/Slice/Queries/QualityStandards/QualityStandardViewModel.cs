namespace MesMicroservice.Application.Queries.QualityStandards;

public class QualityStandardViewModel
{
    public string QualityStandardId { get; set; }
    public string Description { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public EStatus Status { get; set; }
    public List<PropertyViewModel> Properties { get; set; }
    public List<string> MaterialDefinitions { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public QualityStandardViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public QualityStandardViewModel(string qualityStandardId, string description, DateTime validFrom, DateTime validTo, EStatus status, List<PropertyViewModel> properties, List<string> materialDefinitions)
    {
        QualityStandardId = qualityStandardId;
        Description = description;
        ValidFrom = validFrom;
        ValidTo = validTo;
        Status = status;
        Properties = properties;
        MaterialDefinitions = materialDefinitions;
    }
}
