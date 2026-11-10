using Newtonsoft.Json.Linq;

namespace MesMicroservice.Domain.AggregateModels.SegmentAggregate
{
    public interface IProcessSegmentRepository : IRepository<ProcessSegment>
    {
        public Task<ProcessSegment> Add(ProcessSegment processSegment);
        public Task<ProcessSegment?> GetAsync(string processSegmentId);

        public Task<IEnumerable<ProcessSegment>> GetProcessSegments();
        public Task<List<ProcessSegment>> GetListByIdAsync(List<string> processSegmentIds);
        public ProcessSegment Update(ProcessSegment processSegment);
        public Task<bool> ExistsAsync(string processSegmentId);
        public Task DeleteAsync(string processSegmentId);
    }
}
