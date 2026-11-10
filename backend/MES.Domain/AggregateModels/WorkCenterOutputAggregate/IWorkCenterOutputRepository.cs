namespace MesMicroservice.Domain.AggregateModels.WorkCenterOutputAggregate;

public interface IWorkCenterOutputRepository : IRepository<WorkCenterOutput>
{
    public Task<WorkCenterOutput> Add(WorkCenterOutput workCenterOutput);
    public Task<WorkCenterOutput?> GetAsync(int workCenterId, string materialDefinitionId);
    public WorkCenterOutput Update(WorkCenterOutput workCenterOutput);
    public Task<bool> ExistsAsync(int workCenterId, int materialDefinitionId);
    public Task DeleteAsync(int workCenterId, string materialDefinitionId);
}
