namespace MesMicroservice.Application.Mapping.Converters.ProcessSegmentConverter;

public class EquipmentSegmentSpecificationToStringConverter : ITypeConverter<EquipmentSegmentSpecification, string>
{
    public string Convert(EquipmentSegmentSpecification source, string destination, ResolutionContext context)
    {
       return source.ResourceId;
    }
}
