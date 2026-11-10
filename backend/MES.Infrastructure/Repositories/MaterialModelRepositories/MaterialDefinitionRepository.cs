
namespace MesMicroservice.Infrastructure.Repositories.MaterialModelRepositories;
public class MaterialDefinitionRepository : BaseRepository, IMaterialDefinitionRepository
{
    public MaterialDefinitionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<MaterialDefinition> Add(MaterialDefinition materialDefinition)
    {
        if (materialDefinition.IsTransient())
        {
            if (await ExistsAsync(materialDefinition.ResourceId))
            {
                throw new EntityDuplicationException(nameof(MaterialDefinition), materialDefinition.ResourceId);
            }

            return _context.MaterialDefinitions
                .Add(materialDefinition)
                .Entity;
        }
        else
        {
            return materialDefinition;
        }
    }

    public async Task<MaterialDefinition?> GetAsync(string materialDefinitionId)
    {
        var materialDefinition = await _context.MaterialDefinitions
            .Include(x => x.MaterialClass)
            .Include(x => x.QualityStandards)
            .Include(x => x.Properties)
            .Include(x => x.OperationsDefinitions)
            .Include(x => x.SecondaryUnits)
            .FirstOrDefaultAsync(x => x.ResourceId == materialDefinitionId);

        return materialDefinition;
    }


    public async Task<IEnumerable<MaterialDefinition>> GetMaterialDefinitions()
    {
        return await _context.MaterialDefinitions
            .Include(x => x.Properties)
            .Include(x => x.SecondaryUnits) 
            .Include(x => x.MaterialClass)
            .Include(x => x.BillOfMaterials)
            .Include(x => x.OperationsDefinitions)
               .ThenInclude(x => x.OperationsSegments)
            .Include(x => x.QualityStandards)
            .ToListAsync();
    }

    public async Task<List<MaterialDefinition>> GetListByIdAsync(List<string> materialDefinitionIds)
    {
        var materialDefinitions = await _context.MaterialDefinitions
            .Include(x => x.MaterialLots)
            .Include(x => x.QualityStandards)
            .Include(x => x.Properties)
            .Include(x => x.OperationsDefinitions)
            .Where(x => materialDefinitionIds.Contains(x.ResourceId))
            .ToListAsync();

        var notFoundIds = materialDefinitionIds
            .Where(id => !materialDefinitions.Exists(pc => pc.ResourceId == id));

        if (notFoundIds.Any())
        {
            throw new EntitiesNotFoundException(nameof(MaterialDefinition), notFoundIds.ToList());
        }

        return materialDefinitions;
    }

    public MaterialDefinition Update(MaterialDefinition materialDefinition)
    {
        return _context.MaterialDefinitions
                .Update(materialDefinition)
                .Entity;
    }

    public async Task<bool> ExistsAsync(string materialDefinitionId)
    {
        return await _context.MaterialDefinitions
            .AnyAsync(x => x.ResourceId == materialDefinitionId);
    }

    public async Task DeleteAsync(string materialDefinitionId)
    {
        var materialDefinition = await _context.MaterialDefinitions
            .FirstOrDefaultAsync(x => x.ResourceId == materialDefinitionId);

        if (materialDefinition is not null)
        {
            _context.MaterialDefinitions.Remove(materialDefinition);
        }
    }

    public async Task DeleteMaterialUnitAsync(string unitId)
    {
        var materialUnit = await _context.MaterialDefinitions
            .SelectMany(x => x.SecondaryUnits)
            .FirstOrDefaultAsync(x => x.UnitId == unitId);

        if (materialUnit is not null)
        {
            _context.Set<MaterialUnit>().Remove(materialUnit);
        }
    }

    public async Task DeleteOperationAsync(string operationId)
    {
        var operation = await _context.MaterialDefinitions
            .SelectMany(x => x.OperationsDefinitions)
            .FirstOrDefaultAsync(x => x.ResourceId == operationId);

        if (operation is not null)
        {
            _context.Set<OperationsDefinition>().Remove(operation);
        }
    }

    public Task<string> GetNameByIdAsync(string materialDefinitionId)
    {
        return _context.MaterialDefinitions
            .Where(x => x.ResourceId == materialDefinitionId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync()!;
    }

    public Task<List<MaterialDefinition>> GetAll()
    {
        return  _context.MaterialDefinitions
            .AsNoTracking()
            .Include(x => x.Properties)
            .Include(x => x.MaterialClass)
            .Include(x => x.SecondaryUnits)
            .ToListAsync();
    }

    public Task<MaterialDefinition?> GetMaterialByIdAsync(string materialDefinitionId)
    {
        return _context.MaterialDefinitions
            .AsNoTracking()
            .Include(x => x.OperationsDefinitions)
                .ThenInclude(x => x.OperationsSegments)
            .Include(x => x.OperationsDefinitions)
                .ThenInclude(x => x.OperationsMaterialBill)
                    .ThenInclude(x => x.Items)
            .FirstOrDefaultAsync(x => x.ResourceId == materialDefinitionId);
    }

    public Task<List<MaterialDefinition>> GetByClassId(string classId)
    {
        return _context.MaterialDefinitions
            .Include(x => x.MaterialClass)
            .AsNoTracking()
            .Where(x => x.MaterialClass.ResourceId == classId)
            .ToListAsync();
    }
}
