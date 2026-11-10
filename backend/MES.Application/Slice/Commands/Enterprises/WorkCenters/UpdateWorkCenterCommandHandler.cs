namespace MesMicroservice.Application.Commands.Enterprises.WorkCenters;

public class UpdateWorkCenterCommandHandler : IRequestHandler<UpdateWorkCenterCommand, bool>
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public UpdateWorkCenterCommandHandler(IEnterpriseRepository enterpriseRepository)
    {
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(UpdateWorkCenterCommand request, CancellationToken cancellationToken)
    {
        var enterprise = await _enterpriseRepository.GetAsync(request.EnterpriseId) ?? throw new ResourceNotFoundException(nameof(Enterprise), request.EnterpriseId);
        var workCenter = enterprise.Sites
            .SelectMany(x => x.Areas)
            .SelectMany(x => x.WorkCenters)
            .FirstOrDefault(x => x.AbsolutePath == $"{request.EnterpriseId}/{request.SiteId}/{request.AreaId}/{request.WorkCenterId}") ?? throw new ResourceNotFoundException(nameof(WorkCenter), request.WorkCenterId);

        workCenter.Update(request.HierarchyModelId, request.Name, request.WorkCenterType);

        return await _enterpriseRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
