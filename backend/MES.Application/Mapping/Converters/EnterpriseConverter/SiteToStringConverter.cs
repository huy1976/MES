namespace MesMicroservice.Application.Mapping.Converters.EnterpriseConverter;

public class SiteToStringConverter : ITypeConverter<Site, string>
{
    public string Convert(Site source, string destination, ResolutionContext context)
    {
        if (source is null)
        {
            return string.Empty;
        }

        return source.AbsolutePath;
    }
}
