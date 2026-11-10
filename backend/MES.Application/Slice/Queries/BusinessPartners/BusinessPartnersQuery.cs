namespace MesMicroservice.Application.Queries.BusinessPartners;

public class BusinessPartnersQuery : PaginatedQuery, IRequest<IEnumrable<BusinessPartnerViewModel>>
{
    public string? IdStartedWith { get; set; }
}
