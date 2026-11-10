using MesMicroservice.Domain.Enums;

namespace MesMicroservice.Domain.AggregateModels.EquipmentClassAggregate
{
    public class EquipmentDowntimeType : Entity
    {
        public int EquipmentClassId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public EDowntimeType Type { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public EquipmentDowntimeType()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {

        }

        public EquipmentDowntimeType(int equipmentClassId, string name, string description, EDowntimeType type)
        {
            EquipmentClassId = equipmentClassId;
            Name = name;
            Description = description;
            Type = type;
        }
    }
}
