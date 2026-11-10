namespace MesMicroservice.Application.Mapping.Converters;

public class OperationsSegmentToStringConverter : ITypeConverter<OperationsSegment, string>
{
    public string Convert(OperationsSegment source, string destination, ResolutionContext context)
    {
        return source.SegmentId;
    }
}
