namespace MesMicroservice.Application.Queries.BusinessPartners;

public class BusinessPartnerQuery :  IRequest<BusinessPartnerViewModel?>
{
    public string BusinessPartnerId { get; set; }

    public BusinessPartnerQuery(string businessPartnerId)
    {
        BusinessPartnerId = businessPartnerId;
    }
}
