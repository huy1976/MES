
namespace MesMicroservice.Infrastructure.Repositories.MaterialModelRepositories;
public class MaterialLotRepository : BaseRepository, IMaterialLotRepository
{
    public MaterialLotRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<MaterialLot> Add(MaterialLot materialLot)
    {
        if (materialLot.IsTransient())
        {
            if (await ExistsAsync(materialLot.ResourceId))
            {
                throw new EntityDuplicationException(nameof(MaterialLot), materialLot.ResourceId);
            }

            return _context.MaterialLots
                .Add(materialLot)
                .Entity;
        }
        else
        {
            return materialLot;
        }
    }

    public async Task<MaterialLot?> GetAsync(string materialLotId)
    {
        return await _context.MaterialLots
            .Include(x => x.MaterialDefinition)
            .Include(x => x.Unit)
            .FirstOrDefaultAsync(x => x.ResourceId == materialLotId);
    }

    public async Task<IEnumerable<MaterialLot>> GetMaterialLots()
    {
        return await _context.MaterialLots
            .Include(x => x.MaterialDefinition)
            .Include(x => x.Unit)
            .ToListAsync();
    }

    public MaterialLot Update(MaterialLot materialLot)
    {
        return _context.MaterialLots
            .Update(materialLot)
            .Entity;
    }

    public async Task<bool> ExistsAsync(string materialLotId)
    {
        return await _context.MaterialLots
            .AnyAsync(x => x.ResourceId == materialLotId);
    }

    public async Task DeleteAsync(string materialLotId)
    {
        var materialLot = await _context.MaterialLots
            .FirstOrDefaultAsync(x => x.ResourceId == materialLotId);

        if (materialLot is not null)
        {
            _context.MaterialLots.Remove(materialLot);
        }
    }

 
}
