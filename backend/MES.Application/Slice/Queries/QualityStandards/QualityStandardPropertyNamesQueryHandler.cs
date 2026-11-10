
namespace MesMicroservice.Application.Queries.QualityStandards;

public class QualityStandardPropertyNamesQueryHandler : IRequestHandler<QualityStandardPropertyNamesQuery, IEnumerable<string>>
{
    private readonly IQualityStandardRepository _qualityStandardRepository;
    public QualityStandardPropertyNamesQueryHandler(IQualityStandardRepository qualityStandardRepository)
    {
        _qualityStandardRepository = qualityStandardRepository;
    }
    public async Task<IEnumerable<string>> Handle(QualityStandardPropertyNamesQuery request, CancellationToken cancellationToken)
    {
        var qualityStandards = await _qualityStandardRepository.GetQualityStandards();

        var propertyNames = qualityStandards
            .SelectMany(qs => qs.Properties)
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        return propertyNames;
    }
}
