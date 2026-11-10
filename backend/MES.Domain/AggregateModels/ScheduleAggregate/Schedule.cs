namespace MesMicroservice.Domain.AggregateModels.ScheduleAggregate
{
    public class Schedule : IAggregateRoot
    {
        public string Id { get; set; }
        public string Name { get; private set; }
        public WorkDays ActiveDays { get; private set; }
        private readonly List<Shift> _shifts = new List<Shift>();
        public IReadOnlyCollection<Shift> Shifts => _shifts.AsReadOnly();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Schedule() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Schedule(string name, WorkDays activeDays)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Schedule name cannot be empty.");

            Id = Guid.NewGuid().ToString();
            Name = name;
            ActiveDays = activeDays;
        }

        public void AddShift(string shiftName, TimeSpan start, TimeSpan end)
        {
            if (_shifts.Any(s => s.ShiftName.Equals(shiftName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Shift '{shiftName}' already exists.");

            var shift = new Shift(Id, shiftName, start, end);
            _shifts.Add(shift);
        }

        public void RemoveShift(string shiftName)
        {
            var shift = _shifts.FirstOrDefault(s => s.ShiftName.Equals(shiftName, StringComparison.OrdinalIgnoreCase));
            if (shift is null)
                throw new KeyNotFoundException($"Shift '{shiftName}' not found.");

            _shifts.Remove(shift);
        }

        public bool IsWorkingDay(DayOfWeek dayOfWeek)
        {
            var dayFlag = dayOfWeek switch
            {
                DayOfWeek.Monday => WorkDays.Monday,
                DayOfWeek.Tuesday => WorkDays.Tuesday,
                DayOfWeek.Wednesday => WorkDays.Wednesday,
                DayOfWeek.Thursday => WorkDays.Thursday,
                DayOfWeek.Friday => WorkDays.Friday,
                DayOfWeek.Saturday => WorkDays.Saturday,
                DayOfWeek.Sunday => WorkDays.Sunday,
                _ => WorkDays.None
            };

            return ActiveDays.Includes(dayFlag);
        }
    }
}
