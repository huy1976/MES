
namespace MesMicroservice.Application.Queries.Enterprises;

public class SitesQueryHandler : IRequestHandler<SitesQuery, IEnumrable<SiteViewModel>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IMapper _mapper;

    public SitesQueryHandler(ISiteRepository siteRepository, IMapper mapper)
    {
        _siteRepository = siteRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<SiteViewModel>> Handle(SitesQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _siteRepository.GetSites();

        if (request.IdStartedWith is not null)
        {
            queryable = queryable.Where(x => x.HierarchyModelId.StartsWith(request.IdStartedWith));
        }

        if (request.Paginated)
        {
            queryable = queryable
                .OrderBy(x => x.HierarchyModelId)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        int totalItems = queryable.Count();

        var sites = queryable.ToList();

        var queryResult = new IEnumrable<Site>(sites, totalItems);

        return _mapper.Map<IEnumrable<Site>, IEnumrable<SiteViewModel>>(queryResult);
    }
}
