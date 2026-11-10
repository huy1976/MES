
namespace MesMicroservice.Infrastructure.Repositories.OperationsDefinitionRepositories
{
    public class OperationsDefinitionRepository : BaseRepository, IOperationsDefinitionRepository
    {
        public OperationsDefinitionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<OperationsDefinition> Add(OperationsDefinition operationsDefinition)
        {
            if (operationsDefinition.IsTransient())
            {
                if (await ExistsAsync(operationsDefinition.ResourceId))
                {
                    throw new EntityDuplicationException(nameof(MaterialDefinition), operationsDefinition.ResourceId);
                }

                return _context.OperationsDefinitions
                    .Add(operationsDefinition)
                    .Entity;
            }
            else
            {
                return operationsDefinition;
            }
        }

        public async Task DeleteAsync(string operationsDefinitionId)
        {
            var operationsDefinition = await _context.OperationsDefinitions
                .FirstOrDefaultAsync(x => x.ResourceId == operationsDefinitionId);

            if (operationsDefinition is not null)
            {
                _context.OperationsDefinitions.Remove(operationsDefinition);
            }
        }

        public async Task<bool> ExistsAsync(string operationsDefinitionId)
        {
            return await _context.OperationsDefinitions
                .AnyAsync(x => x.ResourceId == operationsDefinitionId);
        }

        public async Task<IEnumerable<OperationsDefinition>> GetAllAsync()
        {
            return await _context.OperationsDefinitions
                  .Include(x => x.MaterialDefinition)
                      .ThenInclude(x => x.BillOfMaterials)
                  .Include(x => x.OperationsSegments)
                      .ThenInclude(x => x.EquipmentSpecifications)
                  .Include(x => x.OperationsSegments)
                      .ThenInclude(x => x.MaterialSpecifications)
                  .Include(x => x.OperationsSegments)
                      .ThenInclude(x => x.PersonnelSpecifications)
                  .Include(x => x.OperationsSegmentDependencies)
                  .Include(x => x.OperationsMaterialBill)
                  .ToListAsync();
        }

        public async Task<OperationsDefinition?> GetAsync(string operationsDefinitionId)
        {
            return await _context.OperationsDefinitions
                .Include(x => x.MaterialDefinition)
                .Include(x => x.OperationsSegments)
                .Include(x => x.OperationsSegmentDependencies)
                .Include(x => x.OperationsMaterialBill)
                    .ThenInclude(x => x.Items)
                .FirstOrDefaultAsync(x => x.ResourceId == operationsDefinitionId);
        }

        public Task<OperationsDefinition?> GetByIdAsync(string operationsDefinitionId)
        {
            return _context.OperationsDefinitions
                .Include(x => x.OperationsSegments)
                .Include(x => x.OperationsSegmentDependencies)
                .Include(x => x.OperationsMaterialBill)
                    .ThenInclude(x => x.Items)
                        
                .FirstOrDefaultAsync(x => x.ResourceId == operationsDefinitionId);
        }

        public async Task<List<OperationsDefinition>> GetListByIdAsync(List<string> operationsDefinitionIds)
        {
            var operationsDefinitions = await _context.OperationsDefinitions
                .Include(x => x.MaterialDefinition)
                .Include(x => x.OperationsSegments)
                .Include(x => x.OperationsSegmentDependencies)
                .Include(x => x.OperationsMaterialBill)
                .Where(x => operationsDefinitionIds.Contains(x.ResourceId))
                .ToListAsync();

            var notFoundIds = operationsDefinitionIds
                .Where(id => !operationsDefinitions.Exists(pc => pc.ResourceId == id));

            if (notFoundIds.Any())
            {
                throw new EntitiesNotFoundException(nameof(MaterialDefinition), notFoundIds.ToList());
            }

            return operationsDefinitions;
        }

        public OperationsDefinition Update(OperationsDefinition operationsDefinition)
        {
            return _context.OperationsDefinitions
                    .Update(operationsDefinition)
                    .Entity;
        }

    }

}
