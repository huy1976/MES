using MesMicroservice.Domain.AggregateModels.MaterialDefinitionAggregate;

namespace MesMicroservice.Application.Mapping.Converters;

public class MaterialDefinitionToStringConverter : ITypeConverter<MaterialDefinition, string>
{
    public string Convert(MaterialDefinition source, string destination, ResolutionContext context)
    {
        return source.ResourceId;
    }
}
