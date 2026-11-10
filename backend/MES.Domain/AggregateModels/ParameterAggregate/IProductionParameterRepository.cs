namespace MesMicroservice.Domain.AggregateModels.ParameterAggregate
{
    public interface IProductionParameterRepository : IRepository<ProductionParameter>
    {
        public Task<ProductionParameter> Add(ProductionParameter productionParameter);
        public Task<ProductionParameter?> GetAsync(string productionParameterId);

        public Task<IEnumerable<ProductionParameter>> GetProductionParameters();
        public Task<List<ProductionParameter>> GetListByIdAsync(List<string> productionParameterIds);
        public ProductionParameter Update(ProductionParameter productionParameter);
        public Task<bool> ExistsAsync(string productionParameterId);
        public Task DeleteAsync(string productionParameterId);
    }
}
