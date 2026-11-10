namespace MesMicroservice.Domain.AggregateModels.BusinessPartnerAggregate;

public interface IBusinessPartnerRepository : IRepository<BusinessPartner>
{
    public Task<BusinessPartner> Add(BusinessPartner businessPartner);
    public Task<BusinessPartner?> GetAsync(string businessPartnerId);

    public Task<IEnumerable<BusinessPartner>> GetBusinessPartners();
    public Task<List<BusinessPartner>> GetListByIdAsync(List<string> businessPartnerIds);
    public BusinessPartner Update(BusinessPartner businessPartner);
    public Task DeleteAsync(string businessPartnerId);
    public Task<bool> ExisAsync(string businessPartnerId);
}
