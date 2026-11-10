global using MesMicroservice.Domain.AggregateModels.ScheduleAggregate;

namespace MesMicroservice.Domain.AggregateModels.WorkOrderAggregate
{
    public class PersonWorkOrder : IAggregateRoot
    {
        public string Id { get; private set; }
        public Person Person { get; private set; }
        public string ClassName { get; private set; }
        public string ShiftName { get; private set; }
        public ICollection<Shift> Shifts { get; private set; } = new List<Shift>();

        public int WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; private set; }
        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private PersonWorkOrder() { }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public PersonWorkOrder(Person person, string className, WorkOrder workOrder, string shiftName)
        {
            Id = Guid.NewGuid().ToString();
            Person = person;
            ClassName = className;
            WorkOrder = workOrder;
            ShiftName = shiftName;
        }

    }
}
