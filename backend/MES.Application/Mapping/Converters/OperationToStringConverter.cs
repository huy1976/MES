using MesMicroservice.Domain.AggregateModels.MaterialDefinitionAggregate;

namespace MesMicroservice.Application.Mapping.Converters;

public class OperationToStringConverter : ITypeConverter<Operation, string>
{
    public string Convert(Operation source, string destination, ResolutionContext context)
    {
        return source.OperationId;
    }
}
