global using MesMicroservice.Domain.AggregateModels.SegmentAggregate;

namespace MesMicroservice.Domain.AggregateModels.ParameterAggregate
{
    public class ProductionParameter : Resource, IAggregateRoot
    {
        public string? Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public EStatus Status { get; set; }

        public ICollection<ProductionParameterProperty> Properties { get; set; } = new List<ProductionParameterProperty>();
        public ICollection<ProcessSegment> ProcessSegments { get; set; } = new List<ProcessSegment>();

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ProductionParameter() : base() { }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ProductionParameter(string productionParameterId, string? description, DateTime validFrom, DateTime validTo, EStatus status, ICollection<ProductionParameterProperty> properties) : base(productionParameterId)
        {
            Description = description;
            ValidFrom = validFrom;
            ValidTo = validTo;
            Status = status;
            Properties = properties;
        }

        public void AddProperties(List<ProductionParameterProperty> properties)
        {
            foreach (var property in properties)
            {
                Properties.Add(property);
            }
        }
    }
}
