namespace MesMicroservice.Application.Mapping.Converters;

public class ResourceToStringConverter : ITypeConverter<Resource, string>
{
    public string Convert(Resource source, string destination, ResolutionContext context)
    {
        return source.ResourceId;
    }
}
