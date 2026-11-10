
using MesMicroservice.Domain.AggregateModels.SegmentAggregate;

namespace MesMicroservice.Infrastructure.Repositories.SegmentModelRepositories
{
    public class ProcessSegmentDependencyRepository : BaseRepository, IProcessSegmentDependencyRepository
    {
        public ProcessSegmentDependencyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ProcessSegmentDependency> Add(ProcessSegmentDependency processSegmentDependency)
        {
            if (processSegmentDependency.IsTransient())
            {
                if (await ExistsAsync(processSegmentDependency.FromSegmentId, processSegmentDependency.ToSegmentId))
                {
                    throw new EntityDuplicationException(nameof(ProcessSegmentDependency), processSegmentDependency.Id.ToString());
                }

                return _context.ProcessSegmentDependencies
                    .Add(processSegmentDependency)
                    .Entity;
            }
            else
            {
                return processSegmentDependency;
            }
        }

        public Task DeleteAsync(string processSegmentDependencyId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync(string fromId, string toId)
        {
            return await _context.ProcessSegmentDependencies
                .AnyAsync(x => x.FromSegmentId == fromId && x.ToSegmentId == toId);
        }

        public Task<ProcessSegmentDependency?> GetAsync(string processSegmentDependencyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProcessSegmentDependency>> GetListByIdAsync(List<string> processSegmentDependencyIds)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProcessSegmentDependency>> GetProcessSegmentDependencies()
        {
            throw new NotImplementedException();
        }

        public ProcessSegmentDependency Update(ProcessSegmentDependency processSegmentDependency)
        {
            throw new NotImplementedException();
        }
    }
}
