namespace MesMicroservice.Application.Mapping.Converters.EnterpriseConverter;

public class AreaToStringConverter : ITypeConverter<Area, string>
{
    public string Convert(Area source, string destination, ResolutionContext context)
    {
        if (source is null)
        {
            return string.Empty;
        }

        return source.AbsolutePath;
    }
}
