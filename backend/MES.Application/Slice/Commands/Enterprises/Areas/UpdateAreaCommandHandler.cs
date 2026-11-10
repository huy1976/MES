namespace MesMicroservice.Application.Commands.Enterprises.Areas;

public class UpdateAreaCommandHandler : IRequestHandler<UpdateAreaCommand, bool>
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public UpdateAreaCommandHandler(IEnterpriseRepository enterpriseRepository)
    {
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(UpdateAreaCommand request, CancellationToken cancellationToken)
    {
        var enterprise = await _enterpriseRepository.GetAsync(request.EnterpriseId) ?? throw new ResourceNotFoundException(nameof(Enterprise), request.EnterpriseId);
        var area = enterprise.Sites
            .SelectMany(x => x.Areas)
            .FirstOrDefault(x => x.AbsolutePath == $"{request.EnterpriseId}/{request.SiteId}/{request.AreaId}") ?? throw new ResourceNotFoundException(nameof(Area), request.AreaId);
        
        area.Update(request.HierarchyModelId, request.Name);

        return await _enterpriseRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
