
namespace MesMicroservice.Application.Queries.BusinessPartners;

public class BusinessPartnersQueryHandler : IRequestHandler<BusinessPartnersQuery, IEnumrable<BusinessPartnerViewModel>>
{
    private readonly IBusinessPartnerRepository _businessPartnerRepository;
    private readonly IMapper _mapper;

    public BusinessPartnersQueryHandler(IBusinessPartnerRepository businessPartnerRepository, IMapper mapper)
    {
        _businessPartnerRepository = businessPartnerRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<BusinessPartnerViewModel>> Handle(BusinessPartnersQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _businessPartnerRepository.GetBusinessPartners();

        if (request.IdStartedWith is not null)
        {
            queryable = queryable.Where(x => x.ResourceId.StartsWith(request.IdStartedWith));
        }

        if (request.Paginated)
        {
            queryable = queryable
                .OrderBy(x => x.ResourceId)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        int totalItems = queryable.Count();

        var businessPartners = queryable.ToList();

        var queryResult = new IEnumrable<BusinessPartner>(businessPartners, totalItems);

        return _mapper.Map<IEnumrable<BusinessPartner>, IEnumrable<BusinessPartnerViewModel>>(queryResult);
    }
}
