namespace MesMicroservice.Application.Mapping.Converters;

public class ProductionParameterToStringConverter : ITypeConverter<ProductionParameter, string>
{
    public string Convert(ProductionParameter source, string destination, ResolutionContext context)
    {
        return source.ResourceId;
    }

}
