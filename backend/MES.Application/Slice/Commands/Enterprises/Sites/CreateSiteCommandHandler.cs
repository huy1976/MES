namespace MesMicroservice.Application.Commands.Enterprises.Sites;

public class CreateSiteCommandHandler : IRequestHandler<CreateSiteCommand, bool>
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public CreateSiteCommandHandler(IEnterpriseRepository enterpriseRepository)
    {
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(CreateSiteCommand request, CancellationToken cancellationToken)
    {
        var enterprise = await _enterpriseRepository.GetAsync(request.EnterpriseId) ?? throw new ResourceNotFoundException(nameof(Enterprise), request.EnterpriseId);

        var areas = new List<Area>();

        if(request.Areas.Count() != 0)
        {
            areas = request.Areas.ConvertAll(x => new Area(
                x.AreaId,
                x.Name
                ));
        }

        var site = new Site(request.SiteId, request.Name);

        foreach (var area in areas)
        {
            site.AddArea(area);
        }

        enterprise.AddSite(site);

        return await _enterpriseRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
