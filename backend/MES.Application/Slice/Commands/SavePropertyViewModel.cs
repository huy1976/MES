namespace MesMicroservice.Application.Commands
{
    public class SavePropertyViewModel
    {
        public string PropertyId { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public string? ValueString { get; set; }
        public EValueType? ValueType { get; set; }
        public string? ValueUnitOfMeasure { get; set; }

        public SavePropertyViewModel(string? propertyId = null, string? description = null, string? valueString = null, 
                                     EValueType? valueType = null, string? valueUnitOfMeasure = null, string? name = null)
        {
            PropertyId = string.IsNullOrWhiteSpace(propertyId)
                ? Guid.NewGuid().ToString()
                : propertyId;

            Description = description;
            ValueString = valueString;
            ValueType = valueType;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            Name = name;
        }
    }
}
