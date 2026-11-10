namespace MesMicroservice.Domain.AggregateModels.ParameterAggregate
{
    public class ProductionParameterProperty : Entity
    {
        public int ProductionParameterId { get; set; }
        public string PropertyId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Value Value { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ProductionParameterProperty() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public ProductionParameterProperty(string propertyId, string description, Value value, string valueUnitOfMeasure, string name)
        {
            PropertyId = propertyId;
            Description = description;
            Value = value;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            Name = name;
        }
    }
}
