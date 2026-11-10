namespace MesMicroservice.Domain.AggregateModels.SegmentAggregate
{
    public interface IProcessSegmentDependencyRepository : IRepository<ProcessSegmentDependency>
    {
        public Task<ProcessSegmentDependency> Add(ProcessSegmentDependency processSegmentDependency);
        public Task<ProcessSegmentDependency?> GetAsync(string processSegmentDependencyId);
        public Task<IEnumerable<ProcessSegmentDependency>> GetProcessSegmentDependencies();
        public Task<List<ProcessSegmentDependency>> GetListByIdAsync(List<string> processSegmentDependencyIds);
        public ProcessSegmentDependency Update(ProcessSegmentDependency processSegmentDependency);
        public Task<bool> ExistsAsync(string fromId, string toId);
        public Task DeleteAsync(string processSegmentDependencyId);
    }
}
