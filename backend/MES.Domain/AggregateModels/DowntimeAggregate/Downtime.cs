using MesMicroservice.Domain.AggregateModels.EquipmentAggregate;
using MesMicroservice.Domain.AggregateModels.EquipmentClassAggregate;

namespace MesMicroservice.Domain.AggregateModels.DowntimeAggregate
{
    public class Downtime : Entity, IAggregateRoot
    {
        public Equipment Equipment { get; private set; }
        public int EquipmentId { get; private set; }
        public DateTime From { get; private set; }
        public DateTime To { get; private set; }
        public EquipmentDowntimeType DowntimeType { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Downtime() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Downtime(Equipment equipment, int equipmentId, DateTime from, DateTime to, EquipmentDowntimeType downtimeType)
        {
            Equipment = equipment;
            EquipmentId = equipmentId;
            From = from;
            To = to;
            DowntimeType = downtimeType;
        }
    }
}
