namespace MesMicroservice.Application.Mapping.Converters.ProcessSegmentConverter;

public class PersonnelSegmentSpecificationToStringConverter : ITypeConverter<PersonnelSegmentSpecification, string>
{
    public string Convert(PersonnelSegmentSpecification source, string destination, ResolutionContext context)
    {
        return source.ResourceId;
    }
}
