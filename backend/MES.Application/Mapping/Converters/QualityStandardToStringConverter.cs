namespace MesMicroservice.Application.Mapping.Converters;

public class QualityStandardToStringConverter : ITypeConverter<QualityStandard, string>
{
    public string Convert(QualityStandard source, string destination, ResolutionContext context)
    {
        return source.ResourceId;
    }
}
