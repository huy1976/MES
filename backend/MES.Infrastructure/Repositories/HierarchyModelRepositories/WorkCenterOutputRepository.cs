namespace MesMicroservice.Infrastructure.Repositories.HierarchyModelRepositories;

public class WorkCenterOutputRepository : BaseRepository, IWorkCenterOutputRepository
{
    public WorkCenterOutputRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<WorkCenterOutput> Add(WorkCenterOutput workCenterOutput)
    {
        if (workCenterOutput.IsTransient())
        {
            if (await ExistsAsync(workCenterOutput.WorkCenterId, workCenterOutput.MaterialDefinitionId))
            {
                throw new EntityDuplicationException(nameof(WorkCenterOutput), $"'{workCenterOutput.WorkCenter.HierarchyModelId}' '{workCenterOutput.MaterialDefinition.ResourceId}'");
            }

            return _context.WorkCenterOutputs
                .Add(workCenterOutput)
                .Entity;
        }
        else
        {
            return workCenterOutput;
        }
    }

    public async Task<WorkCenterOutput?> GetAsync(int workCenterId, string materialDefinitionId)
    {
        return await _context.WorkCenterOutputs
            .Include(x => x.WorkCenter)
            .Include(x => x.MaterialDefinition)
            .Include(x => x.Unit)
            .FirstOrDefaultAsync(x => x.WorkCenterId == workCenterId && x.MaterialDefinition.ResourceId == materialDefinitionId);
    }

    public WorkCenterOutput Update(WorkCenterOutput workCenterOutput)
    {
        return _context.WorkCenterOutputs
                .Update(workCenterOutput)
                .Entity;
    }

    public async Task<bool> ExistsAsync(int workCenterId ,int materialDefinitionId)
    {
        return await _context.WorkCenterOutputs
            .AnyAsync(x => x.WorkCenterId == workCenterId && x.MaterialDefinitionId == materialDefinitionId);
    }

    public async Task DeleteAsync(int workCenterId, string materialDefinitionId)
    {
        var workCenterOutput = await _context.WorkCenterOutputs
            .FirstOrDefaultAsync(x => x.WorkCenterId == workCenterId && x.MaterialDefinition.ResourceId == materialDefinitionId);

        if (workCenterOutput is not null)
        {
            _context.WorkCenterOutputs.Remove(workCenterOutput);
        }
    }
}
