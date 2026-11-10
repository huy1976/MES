namespace MesMicroservice.Domain.AggregateModels.MaterialDefinitionAggregate;
public interface IMaterialDefinitionRepository : IRepository<MaterialDefinition>
{
    public Task<MaterialDefinition> Add(MaterialDefinition materialDefinition);
    public Task<MaterialDefinition?> GetAsync(string materialDefinitionId);
    public Task<MaterialDefinition?> GetMaterialByIdAsync(string materialDefinitionId);
    public Task<IEnumerable<MaterialDefinition>> GetMaterialDefinitions();
    public Task<List<MaterialDefinition>> GetListByIdAsync(List<string> materialDefinitionIds);
    public MaterialDefinition Update(MaterialDefinition materialDefinition);
    public Task<bool> ExistsAsync(string materialDefinitionId);
    public Task DeleteAsync(string materialDefinitionId);
    public Task DeleteMaterialUnitAsync(string unitId);
    public Task DeleteOperationAsync(string operationId);
    public Task<string> GetNameByIdAsync(string materialDefinitionId);
    public Task<List<MaterialDefinition>> GetAll();
    public Task<List<MaterialDefinition>> GetByClassId(string classId);
}
