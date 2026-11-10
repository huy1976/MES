using MesMicroservice.Domain.AggregateModels.DowntimeAggregate;

namespace MesMicroservice.Infrastructure.Repositories
{
    public class DowntimeRepository : BaseRepository, IDowntimeRepository
    {
        public DowntimeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Downtime Add(Downtime downtime)
        {
            return _context.Downtimes
                .Add(downtime)
                .Entity;
        }
    }
}
