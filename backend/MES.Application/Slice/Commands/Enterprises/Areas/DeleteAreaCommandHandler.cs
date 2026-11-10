namespace MesMicroservice.Application.Commands.Enterprises.Areas;

public class DeleteAreaCommandHandler : IRequestHandler<DeleteAreaCommand, bool>
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public DeleteAreaCommandHandler(IEnterpriseRepository enterpriseRepository)
    {
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(DeleteAreaCommand request, CancellationToken cancellationToken)
    {
        var enterprise = await _enterpriseRepository.GetAsync(request.EnterpriseId) ?? throw new ResourceNotFoundException(nameof(Enterprise), request.EnterpriseId);
        var site = enterprise.Sites.FirstOrDefault(x => x.AbsolutePath == $"{request.EnterpriseId}/{request.SiteId}") ?? throw new ResourceNotFoundException(nameof(Site), request.SiteId);

        site.RemoveArea(request.AreaId);

        return await _enterpriseRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}

