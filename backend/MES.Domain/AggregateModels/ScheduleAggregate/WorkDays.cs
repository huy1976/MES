namespace MesMicroservice.Domain.AggregateModels.ScheduleAggregate
{
    public enum WorkDays
    {
        None = 0,
        Monday = 1 << 0,
        Tuesday = 1 << 1,
        Wednesday = 1 << 2,
        Thursday = 1 << 3,
        Friday = 1 << 4,
        Saturday = 1 << 5,
        Sunday = 1 << 6
    }
    public static class WorkDaysExtensions
    {
        public static bool Includes(this WorkDays days, WorkDays day)
            => (days & day) == day;
    }
}
