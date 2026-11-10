using MesMicroservice.Domain.AggregateModels.EquipmentClassAggregate;

namespace MesMicroservice.Application.Mapping.Converters;

public class EquipmentClassToStringConverter : ITypeConverter<EquipmentClass, string>
{
    public string Convert(EquipmentClass source, string destination, ResolutionContext context)
    {
        return source.ResourceId;
    }
}
