namespace MesMicroservice.Domain.AggregateModels.ScheduleAggregate
{
    public interface IScheduleRepository : IRepository<Schedule>
    {
        public Task<Schedule> Add(Schedule schedule);
        public Schedule Update(Schedule schedule);
        public Task DeleteAsync(string scheduleId);
        public Task<bool> ExistAsync(string scheduleId);
    }
}
