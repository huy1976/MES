namespace MesMicroservice.Application.Mapping.Converters;

public class PersonToStringConverter : ITypeConverter<Person, string>
{
    public string Convert(Person source, string destination, ResolutionContext context)
    { 
        return source.ResourceId;
    }
}
