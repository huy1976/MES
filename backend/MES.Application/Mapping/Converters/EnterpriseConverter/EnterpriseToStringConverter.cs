namespace MesMicroservice.Application.Mapping.Converters.EnterpriseConverter;

public class EnterpriseToStringConverter : ITypeConverter<Enterprise, string>
{
    public string Convert(Enterprise source, string destination, ResolutionContext context)
    {
        if (source is null)
        {
            return string.Empty;
        }

        return source.AbsolutePath;
    }
}

