namespace MesMicroservice.Infrastructure.Repositories.HierarchyModelRepositories;

public class SiteRepository : BaseRepository, ISiteRepository
{
    public SiteRepository(ApplicationDbContext context) : base(context)
    {
    }
    public async Task<IEnumerable<Site>> GetSites()
    {
        return await _context.Set<Site>()
            .Include(x => x.Parent)
            .Include(x => x.Areas)
                .ThenInclude(x => x.WorkCenters)
                .ThenInclude(x => x.AssociatedEquipments)
            .ToListAsync();
    }
}
