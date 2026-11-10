

namespace MesMicroservice.Application.Commands.Enterprises.WorkCenters;

public class DeleteWorkCenterOutputCommandHandler : IRequestHandler<DeleteWorkCenterOutputCommand, bool>
{
    private readonly IWorkCenterOutputRepository _workCenterOutputRepository;
    private readonly IEnterpriseRepository _enterpriseRepository;

    public DeleteWorkCenterOutputCommandHandler(IWorkCenterOutputRepository workCenterOutputRepository, IEnterpriseRepository enterpriseRepository)
    {
        _workCenterOutputRepository = workCenterOutputRepository;
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(DeleteWorkCenterOutputCommand request, CancellationToken cancellationToken)
    {
        var workCenter = await GetWorkCenterByAbsolutePath(request.AbsolutePath);
        await _workCenterOutputRepository.DeleteAsync(workCenter.Id, request.MaterialDefinitionId);

        return await _workCenterOutputRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }

    private async Task<WorkCenter> GetWorkCenterByAbsolutePath(string absolutePath)
    {
        var hierarchyModelIds = absolutePath.Split('/');
        var enterprise = await _enterpriseRepository.GetAsync(hierarchyModelIds[0]) ?? throw new ResourceNotFoundException(nameof(Enterprise), hierarchyModelIds[0]);
        var workCenter = enterprise.Sites
            .SelectMany(x => x.Areas)
            .SelectMany(x => x.WorkCenters)
            .FirstOrDefault(x => x.AbsolutePath == absolutePath) ?? throw new ResourceNotFoundException(nameof(WorkCenter), hierarchyModelIds[3]);

        return workCenter;
    }
}
