namespace MesMicroservice.Application.Queries.BusinessPartners
{
    public class CustomerIdsQueryHandler : IRequestHandler<CustomerIdsQuery, IEnumerable<string>>
    {
        private readonly IBusinessPartnerRepository _businessPartnerRepository;

        public CustomerIdsQueryHandler(IBusinessPartnerRepository businessPartnerRepository)
        {
            _businessPartnerRepository = businessPartnerRepository;
        }

        public async Task<IEnumerable<string>> Handle(CustomerIdsQuery request, CancellationToken cancellationToken)
        {
            var businessPartners = await _businessPartnerRepository.GetBusinessPartners();
            // Filter only customers
            var customerIds = businessPartners
                .Where(bp => bp.BusinessPartnerType == EBusinessPartnerType.Customer)
                .Select(bp => bp.ResourceId)
                .ToList();

            return customerIds;
        }
    }

}
