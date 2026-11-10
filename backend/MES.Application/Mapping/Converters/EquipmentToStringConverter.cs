using MesMicroservice.Domain.AggregateModels.EquipmentAggregate;

namespace MesMicroservice.Application.Mapping.Converters;

public class EquipmentToStringConverter : ITypeConverter<Equipment, string>
{
    public string Convert(Equipment source, string destination, ResolutionContext context)
    {
        return source.ResourceId;
    }
}
