namespace MesMicroservice.Domain.AggregateModels.QualityStandardAggregate
{
    public interface IQualityStandardRepository : IRepository<QualityStandard>
    {
        public Task<QualityStandard> Add(QualityStandard qualityStandard);
        public Task<QualityStandard?> GetAsync(string qualityStandardId);

        public Task<IEnumerable<QualityStandard>> GetQualityStandards();
        public Task<List<QualityStandard>> GetListByIdAsync(List<string> qualityStandardIds);
        public QualityStandard Update(QualityStandard qualityStandard);
        public Task<bool> ExistsAsync(string qualityStandardId);
        public Task DeleteAsync(string qualityStandardId);
    }
}
