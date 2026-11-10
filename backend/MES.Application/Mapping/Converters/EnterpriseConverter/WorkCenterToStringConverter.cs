using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Application.Mapping.Converters.EnterpriseConverter;

public class WorkCenterToStringConverter : ITypeConverter<WorkCenter?, string>
{
    public string Convert(WorkCenter? source, string destination, ResolutionContext context)
    {
        if (source is null)
        {
            return string.Empty;
        }
        return source.AbsolutePath;
    }
}
