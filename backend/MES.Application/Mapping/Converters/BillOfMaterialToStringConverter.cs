namespace MesMicroservice.Application.Mapping.Converters;

public class BillOfMaterialToStringConverter : ITypeConverter<BillOfMaterial, string>
{
    public string Convert(BillOfMaterial source, string destination, ResolutionContext context)
    {
        return source.ResourceId;
    }
}
