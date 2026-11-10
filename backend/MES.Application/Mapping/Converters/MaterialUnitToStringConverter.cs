using MesMicroservice.Domain.AggregateModels.MaterialDefinitionAggregate;

namespace MesMicroservice.Application.Mapping.Converters;
public class MaterialUnitToStringConverter : ITypeConverter<Domain.AggregateModels.MaterialDefinitionAggregate.MaterialUnit, string>
{
    public string Convert(Domain.AggregateModels.MaterialDefinitionAggregate.MaterialUnit source, string destination, ResolutionContext context)
    {
        return source.UnitId;
    }
}
