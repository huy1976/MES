namespace MesMicroservice.Application.Mapping.Converters;

public class OperationsDefinitionToStringConverter : ITypeConverter<OperationsDefinition, string>
{
    public string Convert(OperationsDefinition source, string destination, ResolutionContext context)
    {
        return source.ResourceId;
    }
}
