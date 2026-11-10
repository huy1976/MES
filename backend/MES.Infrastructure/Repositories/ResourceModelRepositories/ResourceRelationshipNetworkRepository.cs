
using MesMicroservice.Domain.AggregateModels.ResourceRelationshipNetworkAggregate;

namespace MesMicroservice.Infrastructure.Repositories.ResourceModelRepositories;

public class ResourceRelationshipNetworkRepository : BaseRepository, IResourceRelationshipNetworkRepository
{
    public ResourceRelationshipNetworkRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ResourceRelationshipNetwork> Add(ResourceRelationshipNetwork resourceRelationshipNetwork)
    {
        if (resourceRelationshipNetwork.IsTransient())
        {
            if (await ExistsAsync(resourceRelationshipNetwork.ResourceRelationshipNetworkId))
            {
                throw new EntityDuplicationException(nameof(resourceRelationshipNetwork), resourceRelationshipNetwork.ResourceRelationshipNetworkId);
            }

            return _context.ResourceRelationshipNetworks
                .Add(resourceRelationshipNetwork)
                .Entity;
        }
        else
        {
            return resourceRelationshipNetwork;
        }
    }

    public async Task<ResourceRelationshipNetwork?> GetAsync(string resourceRelationshipNetworkId)
    {
        return await _context.ResourceRelationshipNetworks
            .Include(x => x.Connections)
            .ThenInclude(x => x.Properties)
            .Include(x => x.Connections)
            .ThenInclude(x => x.FromResource)
            .Include(x => x.Connections)
            .ThenInclude(x => x.ToResource)
            .FirstOrDefaultAsync(x => x.ResourceRelationshipNetworkId == resourceRelationshipNetworkId);
    }


    public async Task<IEnumerable<ResourceRelationshipNetwork>> GetResourceRelationshipNetworksById(string resourceRelationshipNetworkId)
    {
        return await _context.ResourceRelationshipNetworks
            .Where(x => x.ResourceRelationshipNetworkId == resourceRelationshipNetworkId)
            .Include(x => x.Connections)
            .ThenInclude(x => x.Properties)
            .Include(x => x.Connections)
            .ThenInclude(x => x.FromResource)
            .Include(x => x.Connections)
            .ThenInclude(x => x.ToResource)
            .ToListAsync();

    }


    public async Task<IEnumerable<ResourceRelationshipNetwork>> GetResourceRelationshipNetworks()
    {
        return await _context.ResourceRelationshipNetworks
           .Include(x => x.Connections)
           .ThenInclude(x => x.Properties)
           .Include(x => x.Connections)
           .ThenInclude(x => x.FromResource)
           .Include(x => x.Connections)
           .ThenInclude(x => x.ToResource)
           .ToListAsync();
    }


    public ResourceRelationshipNetwork Update(ResourceRelationshipNetwork resourceRelationshipNetwork)
    {
        return _context.ResourceRelationshipNetworks
            .Update(resourceRelationshipNetwork)
            .Entity;
    }

    public async Task<bool> ExistsAsync(string resourceRelationshipNetworkId)
    {
        return await _context.ResourceRelationshipNetworks
            .AnyAsync(x => x.ResourceRelationshipNetworkId == resourceRelationshipNetworkId);
    }

    public async Task DeleteAsync(string resourceRelationshipNetworkId)
    {
        var resourceRelationshipNetwork = await _context.ResourceRelationshipNetworks
            .FirstOrDefaultAsync(x => x.ResourceRelationshipNetworkId == resourceRelationshipNetworkId);

        if (resourceRelationshipNetwork is not null)
        {
            _context.ResourceRelationshipNetworks.Remove(resourceRelationshipNetwork);
        }
    }

}
