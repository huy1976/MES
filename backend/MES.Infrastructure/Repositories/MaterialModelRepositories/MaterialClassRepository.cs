
namespace MesMicroservice.Infrastructure.Repositories.MaterialModelRepositories;

public class MaterialClassRepository : BaseRepository, IMaterialClassRepository
{
    public MaterialClassRepository(ApplicationDbContext context) : base(context)
    { 
    }

    public async Task<MaterialClass> AddAsync(MaterialClass materialClass)
    {
        if (materialClass.IsTransient())
        {
            if (await CheckExistenceAsync(materialClass.ResourceId))
            {
                throw new EntityDuplicationException(nameof(MaterialClass), materialClass.ResourceId);
            }

            return _context.MaterialClasses
                .Add(materialClass)
                .Entity;
        }
        else
        {
            return materialClass;
        }
    }

    public async Task<MaterialClass?> GetAsync(string materialClassId)
    {
        return await _context.MaterialClasses
            .Include(x => x.MaterialDefinitions)
            .FirstOrDefaultAsync(x => x.ResourceId == materialClassId);
    }

    public async Task<IEnumerable<MaterialClass>> GetMaterialClasses()
    {
        return await _context.MaterialClasses
            .Include(x => x.MaterialDefinitions)
            .Include(x => x.Properties)
            .ToListAsync();
    }


    public async Task<List<MaterialClass>> GetListByIdsAsync(List<string> materialClassIds)
    {
        var materialClasses = await _context.MaterialClasses
            .Include(x => x.MaterialDefinitions)
            .Where(x => materialClassIds.Contains(x.ResourceId))
            .ToListAsync();

        var notFoundIds = materialClassIds
            .Where(id => !materialClasses.Exists(pc => pc.ResourceId == id));

        if (notFoundIds.Any())
        {
            throw new EntitiesNotFoundException(nameof(MaterialClass), notFoundIds.ToList());
        }

        return materialClasses;
    }

    public MaterialClass Update(MaterialClass materialClass)
    {
        return _context.MaterialClasses
            .Update(materialClass)
            .Entity;
    }

    public async Task<bool> CheckExistenceAsync(string materialClassId)
    {
        return await _context.MaterialClasses
            .AnyAsync(x => x.ResourceId == materialClassId);
    }

    public async Task Delete(string materialClassId)
    {
        var materialClass = await _context.MaterialClasses
            .FirstOrDefaultAsync(x => x.ResourceId == materialClassId);

        if (materialClass is not null)
        {
            _context.MaterialClasses.Remove(materialClass);
        }
    }

    
}
