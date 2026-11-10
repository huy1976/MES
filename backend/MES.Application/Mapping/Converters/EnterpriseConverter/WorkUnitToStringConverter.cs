namespace MesMicroservice.Application.Mapping.Converters.EnterpriseConverter;

public class WorkUnitToStringConverter : ITypeConverter<WorkUnit, string>
{
    public string Convert(WorkUnit source, string destination, ResolutionContext context)
    {
        if (source is null)
        {
            return string.Empty;
        }

        return source.AbsolutePath;
    }
}
