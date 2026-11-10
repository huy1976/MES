
namespace MesMicroservice.Infrastructure.Repositories.HierarchyModelRepositories;

public class EnterpriseRepository : BaseRepository, IEnterpriseRepository
{
    public EnterpriseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Enterprise> Add(Enterprise enterprise)
    {
        if (enterprise.IsTransient())
        {
            if (await ExistsAsync(enterprise.HierarchyModelId))
            {
                throw new EntityDuplicationException(nameof(Enterprise), enterprise.HierarchyModelId);
            }

            return _context.Enterprises
                .Add(enterprise)
                .Entity;
        }
        else
        {
            return enterprise;
        }
    }

    public async Task<Enterprise?> GetAsync(string enterpriseId)
    {
        return await _context.Enterprises
            .Include(x => x.Sites)
            .ThenInclude(x => x.Areas)
            .ThenInclude(x => x.WorkCenters)
            .ThenInclude(x => x.WorkUnits)
            .FirstOrDefaultAsync(x => x.HierarchyModelId == enterpriseId);
    }

    public async Task<IEnumerable<Enterprise>> GetEnterprises()
    {
        return await _context.Enterprises
            .Include(x => x.Sites)
            .ThenInclude(x => x.Areas)
            .ThenInclude(x => x.WorkCenters)
            .ThenInclude(x => x.WorkUnits)
            .ToListAsync();
    }

    public async Task<List<Enterprise>> GetListByIdAsync(List<string> enterpriseIds)
    {
        var enterprises = await _context.Enterprises
            .Include(x => x.Sites)
            .ThenInclude(x => x.Areas)
            .ThenInclude(x => x.WorkCenters)
            .ThenInclude(x => x.WorkUnits)
            .Where(x => enterpriseIds.Contains(x.HierarchyModelId))
            .ToListAsync();

        var notFoundIds = enterpriseIds
            .Where(id => !enterprises.Exists(pc => pc.HierarchyModelId == id));

        if (notFoundIds.Any())
        {
            throw new EntitiesNotFoundException(nameof(Enterprise), notFoundIds.ToList());
        }

        return enterprises;
    }

    public Enterprise Update(Enterprise enterprise)
    {
        return _context.Enterprises
                .Update(enterprise)
                .Entity;
    }

    public async Task<bool> ExistsAsync(string enterpriseId)
    {
        return await _context.Enterprises
            .AnyAsync(x => x.HierarchyModelId == enterpriseId);
    }

    public async Task DeleteAsync(string enterpriseId)
    {
        var enterprise = await _context.Enterprises
            .FirstOrDefaultAsync(x => x.HierarchyModelId == enterpriseId);

        if (enterprise is not null)
        {
            _context.Enterprises.Remove(enterprise);
        }
    }


}
