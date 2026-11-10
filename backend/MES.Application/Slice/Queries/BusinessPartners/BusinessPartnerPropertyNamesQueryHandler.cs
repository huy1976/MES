
namespace MesMicroservice.Application.Queries.BusinessPartners;

public class BusinessPartnerPropertyNamesQueryHandler : IRequestHandler<BusinessPartnerPropertyNamesQuery, IEnumerable<string>>
{
    private readonly IBusinessPartnerRepository _businessPartnerRepository;

    public BusinessPartnerPropertyNamesQueryHandler(IBusinessPartnerRepository businessPartnerRepository)
    {
        _businessPartnerRepository = businessPartnerRepository;
    }

    public async Task<IEnumerable<string>> Handle(BusinessPartnerPropertyNamesQuery request, CancellationToken cancellationToken)
    {
        var businessPartners = await _businessPartnerRepository.GetBusinessPartners();

        var propertyNames = businessPartners
            .SelectMany(bp => bp.Properties)
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        return propertyNames;
    }
}
