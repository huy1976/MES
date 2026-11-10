namespace MesMicroservice.Application.Mapping.Converters.ProcessSegmentConverter;

public class MaterialSegmentSpecificationToStringConverter : ITypeConverter<MaterialSegmentSpecification, string>
{
    public string Convert(MaterialSegmentSpecification source, string destination, ResolutionContext context)
    {
        return source.ResourceId;
    }
}
