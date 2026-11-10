namespace MesMicroservice.Application.Mapping.Converters;

public class PersonnelClassToStringConverter : ITypeConverter<PersonnelClass, string>
{
    public string Convert(PersonnelClass source, string destination, ResolutionContext context)
    {
        return source.ResourceId;
    }

}
