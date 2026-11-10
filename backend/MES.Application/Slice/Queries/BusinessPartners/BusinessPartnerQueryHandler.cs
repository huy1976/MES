
namespace MesMicroservice.Application.Queries.BusinessPartners;

public class BusinessPartnerQueryHandler : IRequestHandler<BusinessPartnerQuery, BusinessPartnerViewModel?>
{
    private readonly IBusinessPartnerRepository _businessPartnerRepository;
    private readonly IMapper _mapper;

    public BusinessPartnerQueryHandler(IBusinessPartnerRepository businessPartnerRepository, IMapper mapper)
    {
        _businessPartnerRepository = businessPartnerRepository;
        _mapper = mapper;
    }

    public async Task<BusinessPartnerViewModel?> Handle(BusinessPartnerQuery request, CancellationToken cancellationToken)
    {
        var businessParter = await _businessPartnerRepository.GetAsync(request.BusinessPartnerId);

       return _mapper.Map<BusinessPartner?, BusinessPartnerViewModel?>(businessParter);
    }
}
