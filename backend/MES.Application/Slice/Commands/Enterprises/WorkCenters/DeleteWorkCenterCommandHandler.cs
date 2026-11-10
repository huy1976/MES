namespace MesMicroservice.Application.Commands.Enterprises.WorkCenters;

public class DeleteWorkCenterCommandHandler : IRequestHandler<DeleteWorkCenterCommand, bool>
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public DeleteWorkCenterCommandHandler(IEnterpriseRepository enterpriseRepository)
    {
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(DeleteWorkCenterCommand request, CancellationToken cancellationToken)
    {
        var enterprise = await _enterpriseRepository.GetAsync(request.EnterpriseId) ?? throw new ResourceNotFoundException(nameof(Enterprise), request.EnterpriseId);
        var area = enterprise.Sites
            .SelectMany(x => x.Areas)
            .FirstOrDefault(x => x.AbsolutePath == $"{request.EnterpriseId}/{request.SiteId}/{request.AreaId}") ?? throw new ResourceNotFoundException(nameof(Area), request.AreaId);

        area.RemoveWorkCenter(request.WorkCenterId);

        return await _enterpriseRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}

