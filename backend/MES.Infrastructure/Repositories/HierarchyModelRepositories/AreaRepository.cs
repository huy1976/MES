
namespace MesMicroservice.Infrastructure.Repositories.HierarchyModelRepositories;

public class AreaRepository : BaseRepository, IAreaRepository
{
    public AreaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Area>> GetAreas()
    {
        return await _context.Set<Area>().ToListAsync();
    }
}
