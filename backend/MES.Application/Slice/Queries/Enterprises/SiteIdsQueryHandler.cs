
namespace MesMicroservice.Application.Queries.Enterprises;

public class SiteIdsQueryHandler : IRequestHandler<SiteIdsQuery, IEnumerable<string>>
{
    private readonly ISiteRepository _siteRepository;

    public SiteIdsQueryHandler(ISiteRepository siteRepository)
    {
        _siteRepository = siteRepository;
    }

    public async Task<IEnumerable<string>> Handle(SiteIdsQuery request, CancellationToken cancellationToken)
    {
        var sites = await _siteRepository.GetSites();

        return sites.Select(x => x.HierarchyModelId).ToList();
    }
}
