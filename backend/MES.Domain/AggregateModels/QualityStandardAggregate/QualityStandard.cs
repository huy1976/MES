namespace MesMicroservice.Domain.AggregateModels.QualityStandardAggregate
{
    public class QualityStandard : Resource, IAggregateRoot
    {
        public string Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public EStatus Status { get; set; }
        public ICollection<QualityStandardProperty> Properties { get; set; } = new List<QualityStandardProperty>();
        public ICollection<MaterialDefinition> MaterialDefinitions { get; set; } = new List<MaterialDefinition>();
        public ICollection<ManufacturingOrder> ManufacturingOrders { get; set; } = new List<ManufacturingOrder>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private QualityStandard() : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public QualityStandard(string qualityStandardId, string description, DateTime validFrom, DateTime validTo, EStatus status, 
                               ICollection<QualityStandardProperty> properties, ICollection<MaterialDefinition> materialDefinitions) : base(qualityStandardId)
        {
            Description = description;
            ValidFrom = validFrom;
            ValidTo = validTo;
            Status = status;
            Properties = properties;
            MaterialDefinitions = materialDefinitions;
        }
    }
}
