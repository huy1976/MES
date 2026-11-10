namespace MesMicroservice.Application.Commands.Enterprises.Sites;

public class UpdateSiteCommandHandler : IRequestHandler<UpdateSiteCommand, bool>
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public UpdateSiteCommandHandler(IEnterpriseRepository enterpriseRepository)
    {
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(UpdateSiteCommand request, CancellationToken cancellationToken)
    {
        var enterprise = await _enterpriseRepository.GetAsync(request.EnterpriseId) ?? throw new ResourceNotFoundException(nameof(Enterprise), request.EnterpriseId);
        var site = enterprise.Sites.FirstOrDefault(x => x.AbsolutePath == $"{request.EnterpriseId}/{request.SiteId}") ?? throw new ResourceNotFoundException(nameof(Site), request.SiteId);

        site.Update(request.HierarchyModelId, request.Name);

        return await _enterpriseRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
