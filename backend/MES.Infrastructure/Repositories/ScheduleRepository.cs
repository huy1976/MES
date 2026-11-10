using MesMicroservice.Domain.AggregateModels.ScheduleAggregate;

namespace MesMicroservice.Infrastructure.Repositories
{
    public class ScheduleRepository : BaseRepository, IScheduleRepository
    {
        public ScheduleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Schedule> Add(Schedule schedule)
        {
            if (await ExistAsync(schedule.Name))
            {
                throw new EntityDuplicationException(nameof(Schedule), schedule.Name);
            }

            return _context.Schedules
                .Add(schedule)
                .Entity;
        }

        public Task DeleteAsync(string scheduleId)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> ExistAsync(string scheduleId)
        {
            return await _context.Schedules
                .AnyAsync(x => x.Name == scheduleId);
        }

        public Schedule Update(Schedule schedule)
        {
            throw new NotImplementedException();
        }
    }
}
