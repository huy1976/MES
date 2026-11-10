namespace MesMicroservice.Domain.AggregateModels.ScheduleAggregate
{
    public class Shift
    {
        public string ScheduleId { get; private set; }
        public string Id { get; private set; }
        public string ShiftName { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }

        public ICollection<PersonWorkOrder> PersonWorkOrders { get; private set; } = new List<PersonWorkOrder>();

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Shift() { }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Shift(string scheduleId, string shiftName, TimeSpan startTime, TimeSpan endTime)
        {
            Id = Guid.NewGuid().ToString();

            if (string.IsNullOrWhiteSpace(shiftName))
                throw new ArgumentException("Shift name cannot be empty.");

            if (endTime <= startTime)
                throw new ArgumentException("EndTime must be after StartTime.");

            ScheduleId = scheduleId;
            ShiftName = shiftName;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
