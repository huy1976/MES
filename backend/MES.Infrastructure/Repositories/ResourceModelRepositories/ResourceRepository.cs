namespace MesMicroservice.Infrastructure.Repositories.ResourceModelRepositories;

public class ResourceRepository : BaseRepository, IResourceRepository
{
    public ResourceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Resource?> GetAsync(string resourceId)
    {
        return await _context.Resources
            .FirstOrDefaultAsync(x => x.ResourceId == resourceId);
    }
}
