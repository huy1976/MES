namespace MesMicroservice.Application.Commands.Enterprises.WorkUnits;

public class CreateWorkUnitCommandHandler : IRequestHandler<CreateWorkUnitCommand, bool>
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public CreateWorkUnitCommandHandler(IEnterpriseRepository enterpriseRepository)
    {
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(CreateWorkUnitCommand request, CancellationToken cancellationToken)
    {
        var enterprise = await _enterpriseRepository.GetAsync(request.EnterpriseId) ?? throw new ResourceNotFoundException(nameof(Enterprise), request.EnterpriseId);
        var workCenter = enterprise.Sites
            .SelectMany(x => x.Areas)
            .SelectMany(x => x.WorkCenters)
            .FirstOrDefault(x => x.AbsolutePath == $"{request.EnterpriseId}/{request.SiteId}/{request.AreaId}/{request.WorkCenterId}") ?? throw new ResourceNotFoundException(nameof(WorkCenter), request.WorkCenterId);
        var workUnit = new WorkUnit(request.WorkUnitId, request.Name);

        workCenter.AddWorkUnit(workUnit);

        return await _enterpriseRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
