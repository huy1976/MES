namespace MesMicroservice.Application.Queries.BusinessPartners;

public class BusinessPartnerViewModel
{
    public string BusinessPartnerId { get; set; }
    public string Name { get; set; }
    public EBusinessPartnerType BusinessPartnerType { get; set; }
    public List<PropertyViewModel> Properties { get; private set; } 

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public BusinessPartnerViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }
    public BusinessPartnerViewModel(string businessPartnerId, string name, EBusinessPartnerType businessPartnerType, List<PropertyViewModel> properties)
    {
        BusinessPartnerId = businessPartnerId;
        Name = name;
        BusinessPartnerType = businessPartnerType;
        Properties = properties;
    }
}