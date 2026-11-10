
namespace MesMicroservice.Infrastructure.Repositories.EquipmentModelRepositories;
public class EquipmentClassRepository : BaseRepository, IEquipmentClassRepository
{
    public EquipmentClassRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<EquipmentClass> AddAsync(EquipmentClass equipmentClass)
    {
        if (equipmentClass.IsTransient())
        {
            if (await CheckExistenceAsync(equipmentClass.ResourceId))
            {
                throw new EntityDuplicationException(nameof(EquipmentClass), equipmentClass.ResourceId);
            }

            return _context.EquipmentClasses
                .Add(equipmentClass)
                .Entity;
        }
        else
        {
            return equipmentClass;
        }
    }

    public async Task<EquipmentClass?> GetAsync(string equipmentClassId)
    {
        return await _context.EquipmentClasses
            .Include(x => x.Equipments)
            .ThenInclude(x => x.HierarchyModel)
            .FirstOrDefaultAsync(x => x.ResourceId == equipmentClassId);
    }

    public async Task<IEnumerable<EquipmentClass>> GetEquipmentClasses()
    {
        return await _context.EquipmentClasses
            .Include(x => x.Equipments)
            .ThenInclude(x => x.HierarchyModel)
            .Include(x => x.Properties)
            .ToListAsync();
    }


    public async Task<List<EquipmentClass>> GetListByIdsAsync(List<string> equipmentClassIds)
    {
        var equipmentClasses = await _context.EquipmentClasses
            .Include(x => x.Equipments)
            .Include(x => x.Equipments)
            .ThenInclude(x => x.HierarchyModel)
            .Where(x => equipmentClassIds.Contains(x.ResourceId))
            .ToListAsync();

        var notFoundIds = equipmentClassIds
            .Where(id => !equipmentClasses.Exists(pc => pc.ResourceId == id));

        if (notFoundIds.Any())
        {
            throw new EntitiesNotFoundException(nameof(EquipmentClass), notFoundIds.ToList());
        }

        return equipmentClasses;
    }

    public EquipmentClass Update(EquipmentClass equipmentClass)
    {
        return _context.EquipmentClasses
            .Update(equipmentClass)
            .Entity;
    }

    public async Task<bool> CheckExistenceAsync(string equipmentClassId)
    {
        return await _context.EquipmentClasses
            .AnyAsync(x => x.ResourceId == equipmentClassId);
    }

    public async Task Delete(string equipmentClassId)
    {
        var equipmentClass = await _context.EquipmentClasses
            .FirstOrDefaultAsync(x => x.ResourceId == equipmentClassId);

        if (equipmentClass is not null)
        {
            _context.EquipmentClasses.Remove(equipmentClass);
        }
    }

   
}
