namespace MesMicroservice.Application.Mapping.Converters;

public class EquipmentCapabilityToStringConverter : ITypeConverter<EquipmentCapability, string>
{
    public string Convert(EquipmentCapability source, string destination, ResolutionContext context)
    {
        return source.ResourceId;
    }
}

