namespace MesMicroservice.Application.Commands.Enterprises.WorkUnits;

public class UpdateWorkUnitCommandHandler : IRequestHandler<UpdateWorkUnitCommand, bool>
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public UpdateWorkUnitCommandHandler(IEnterpriseRepository enterpriseRepository)
    {
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(UpdateWorkUnitCommand request, CancellationToken cancellationToken)
    {
        var enterprise = await _enterpriseRepository.GetAsync(request.EnterpriseId) ?? throw new ResourceNotFoundException(nameof(Enterprise), request.EnterpriseId);
        var workUnit = enterprise.Sites
            .SelectMany(x => x.Areas)
            .SelectMany(x => x.WorkCenters)
            .SelectMany(x => x.WorkUnits)
            .FirstOrDefault(x => x.AbsolutePath == $"{request.EnterpriseId}/{request.SiteId}/{request.AreaId}/{request.WorkCenterId}/{request.WorkUnitId}") ?? throw new ResourceNotFoundException(nameof(WorkUnit), request.WorkUnitId);

        workUnit.Update(request.HierarchyModelId, request.Name);

        return await _enterpriseRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
