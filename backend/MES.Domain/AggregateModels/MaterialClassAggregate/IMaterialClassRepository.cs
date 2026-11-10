namespace MesMicroservice.Domain.AggregateModels.MaterialClassAggregate;

public interface IMaterialClassRepository : IRepository<MaterialClass>
{
    public Task<MaterialClass> AddAsync(MaterialClass materialClass);
    public Task<MaterialClass?> GetAsync(string materialClassId);

    public Task<IEnumerable<MaterialClass>> GetMaterialClasses();
    public MaterialClass Update(MaterialClass materialClass);
    public Task<List<MaterialClass>> GetListByIdsAsync(List<string> materialClassIds);
    public Task<bool> CheckExistenceAsync(string materialClassId);
    public Task Delete(string materialClassId);
}
