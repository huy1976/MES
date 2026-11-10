
namespace MesMicroservice.Infrastructure.Repositories.BusinessPartnerRepositories;

public class BusinessPartnerRepository : BaseRepository, IBusinessPartnerRepository
{
    public BusinessPartnerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<BusinessPartner> Add(BusinessPartner businessPartner)
    {
        if (businessPartner.IsTransient())
        {
            if (await ExisAsync(businessPartner.ResourceId))
            {
                throw new EntityDuplicationException($"BusinessPartner with id {businessPartner.ResourceId} already exists.");
            }
            return _context.BusinessPartners
                .Add(businessPartner)
                .Entity;
        }
        else
        {
            return businessPartner;
        }
    }

    public async Task DeleteAsync(string businessPartnerId)
    {
        var qualityStandard = await _context.QualityStandards
            .FirstOrDefaultAsync(x => x.ResourceId == businessPartnerId);

        if (qualityStandard is not null)
        {
            _context.QualityStandards.Remove(qualityStandard);
        }
    }

    public async Task<bool> ExisAsync(string businessPartnerId)
    {
        return await _context.BusinessPartners
            .AnyAsync(x => x.ResourceId == businessPartnerId);
    }

    public async Task<BusinessPartner?> GetAsync(string businessPartnerId)
    {
        return await _context.BusinessPartners
            .Include(x => x.Properties)
            .FirstOrDefaultAsync(x => x.ResourceId == businessPartnerId);
    }

    public async Task<IEnumerable<BusinessPartner>> GetBusinessPartners()
    {
        return await _context.BusinessPartners
          .Include(x => x.Properties)
          .ToListAsync();
    }

    public async Task<List<BusinessPartner>> GetListByIdAsync(List<string> businessPartnerIds)
    {
        var businessPartner = await _context.BusinessPartners
            .Include(x => x.Properties)
            .Where(x => businessPartnerIds.Contains(x.ResourceId))
            .ToListAsync();

        var notFoundIds = businessPartnerIds
            .Where(id => !businessPartner.Exists(pc => pc.ResourceId == id));

        if (notFoundIds.Any())
        {
            throw new EntitiesNotFoundException(nameof(MaterialDefinition), notFoundIds.ToList());
        }

        return businessPartner;
    }

    public BusinessPartner Update(BusinessPartner businessPartner)
    {
        return _context.BusinessPartners
                .Update(businessPartner)
                .Entity;
    }
}
