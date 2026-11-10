using MesMicroservice.Domain.AggregateModels.ManufacturingOrderAggregate;

namespace MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

public interface IEnterpriseRepository : IRepository<Enterprise>
{
    public Task<Enterprise> Add(Enterprise enterprise);
    public Task<Enterprise?> GetAsync(string enterpriseId);
    public Task<IEnumerable<Enterprise>> GetEnterprises();
    public Task<List<Enterprise>> GetListByIdAsync(List<string> enterpriseIds);
    public Enterprise Update(Enterprise enterprise);
    public Task<bool> ExistsAsync(string enterpriseId);
    public Task DeleteAsync(string enterpriseId);
}
