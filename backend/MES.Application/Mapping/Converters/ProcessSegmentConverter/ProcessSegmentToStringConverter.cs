namespace MesMicroservice.Application.Mapping.Converters;

public class ProcessSegmentToStringConverter : ITypeConverter<ProcessSegment, string>
{
    public string Convert(ProcessSegment source, string destination, ResolutionContext context)
    {
        return source.ResourceId;
    }
}
