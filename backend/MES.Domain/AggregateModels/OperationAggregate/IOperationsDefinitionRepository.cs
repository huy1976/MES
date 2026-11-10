namespace MesMicroservice.Domain.AggregateModels.OperationAggregate
{
    public interface IOperationsDefinitionRepository : IRepository<OperationsDefinition>
    {
        public Task<OperationsDefinition> Add(OperationsDefinition operationsDefinition);
        public Task<OperationsDefinition?> GetAsync(string operationsDefinitionId);
        public Task<List<OperationsDefinition>> GetListByIdAsync(List<string> operationsDefinitionIds);
        public OperationsDefinition Update(OperationsDefinition operationsDefinition);
        public Task<bool> ExistsAsync(string operationsDefinitionId);
        public Task DeleteAsync(string operationsDefinitionId);
        public Task<IEnumerable<OperationsDefinition>> GetAllAsync();
        public Task<OperationsDefinition?> GetByIdAsync(string operationsDefinitionId);
    }
}
