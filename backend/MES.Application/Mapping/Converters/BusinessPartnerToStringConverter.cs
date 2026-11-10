namespace MesMicroservice.Application.Mapping.Converters;

internal class BusinessPartnerToStringConverter : ITypeConverter<BusinessPartner?, string?>
{
    public string? Convert(BusinessPartner? source, string? destination, ResolutionContext context)
    {
        return source?.ResourceId;
    }
}
