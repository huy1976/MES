


namespace MesMicroservice.Infrastructure.Repositories.EquipmentModelRepositories;
public class EquipmentRepository : BaseRepository, IEquipmentRepository
{
    public EquipmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Equipment> Add(Equipment equipment)
    {
        if (equipment.IsTransient())
        {
            if (await ExistsAsync(equipment.ResourceId))
            {
                throw new EntityDuplicationException($"Equipment with id {equipment.Id} already exists.");
            }

            return _context.Equipments
                .Add(equipment)
                .Entity;
        }
        else
        {
            return equipment;
        }
    }

    public async Task<Equipment?> GetAsync(string equipmentId)
    {
        return await _context.Equipments
            .Include(x => x.EquipmentClass)
            .Include(x => x.HierarchyModel)
            .Include(x => x.EquipmentCapabilities)
                .ThenInclude(x => x.Properties)
            .FirstOrDefaultAsync(x => x.ResourceId == equipmentId);
    }

    public async Task<IEnumerable<Equipment>> GetEquipments()
    {
        return await _context.Equipments
            .Include(x => x.EquipmentClass)
            .Include(x => x.HierarchyModel)
            .Include(x => x.EquipmentCapabilities)
            .ToListAsync();
    }

    public Task<List<Equipment>> GetAllAsync()
    {
        return _context.Equipments
            .Include(x => x.EquipmentClass)
            .Include(x => x.Properties)
            .Include(x => x.EquipmentCapabilities)
                .Include(x => x.Properties)
            .Include(x => x.HierarchyModel).ToListAsync();
    }


    public Equipment Update(Equipment equipment)
    {
        return _context.Equipments
                .Update(equipment)
                .Entity;
    }

    public async Task<bool> ExistsAsync(string equipmentId)
    {
        return await _context.Equipments
            .AnyAsync(x => x.ResourceId == equipmentId);
    }

    public async Task DeleteAsync(string equipmentId)
    {
        var equipment = await _context.Equipments
            .FirstOrDefaultAsync(x => x.ResourceId == equipmentId);

        if (equipment is not null)
        {
            _context.Equipments.Remove(equipment);
        }
    }

    public async Task<WorkCenter> GetWorkCenterByEquipmentId(string workCenterId)
    {
        var workCenter = await _context.Enterprises
            .Include(e => e.Sites)
                .ThenInclude(s => s.Areas)
                    .ThenInclude(a => a.WorkCenters)
            .SelectMany(e => e.Sites)
            .SelectMany(s => s.Areas)
            .SelectMany(a => a.WorkCenters)
            .FirstOrDefaultAsync(wc => wc.HierarchyModelId == workCenterId);


        return workCenter ?? new();
    }

    public async Task<List<Equipment>> GetAll()
    {
        return await _context.Equipments
            .Include(x => x.EquipmentClass)
            .Include(x => x.HierarchyModel).ToListAsync();
    }
}
