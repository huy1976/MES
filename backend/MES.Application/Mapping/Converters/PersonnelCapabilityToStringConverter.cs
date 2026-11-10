namespace MesMicroservice.Application.Mapping.Converters;

public class PersonnelCapabilityToStringConverter : ITypeConverter<PersonnelCapability, string>
{
    public string Convert(PersonnelCapability source, string destination, ResolutionContext context)
    {
        return source.ResourceId;
    }
}
