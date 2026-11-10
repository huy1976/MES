using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Application.Mapping.Converters.EnterpriseConverter;

public class HierarchyModelToStringConverter : ITypeConverter<HierarchyModel, string?>
{
    public string? Convert(HierarchyModel? source, string? destination, ResolutionContext context)
    {
        if (source is null)
        {
            return string.Empty;
        }

        return source.AbsolutePath;
    }
}
