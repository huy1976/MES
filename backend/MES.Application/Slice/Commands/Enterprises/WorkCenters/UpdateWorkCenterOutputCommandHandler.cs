using MesMicroservice.Domain.AggregateModels.WorkCenterOutputAggregate;

namespace MesMicroservice.Application.Commands.Enterprises.WorkCenters;

public class UpdateWorkCenterOutputCommandHandler : IRequestHandler<UpdateWorkCenterOutputCommand, bool>
{
    private readonly IWorkCenterOutputRepository _workCenterOutputRepository;
    private readonly IEnterpriseRepository _enterpriseRepository;
    private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

    public UpdateWorkCenterOutputCommandHandler(IWorkCenterOutputRepository workCenterOutputRepository, IEnterpriseRepository enterpriseRepository, IMaterialDefinitionRepository materialDefinitionRepository)
    {
        _workCenterOutputRepository = workCenterOutputRepository;
        _enterpriseRepository = enterpriseRepository;
        _materialDefinitionRepository = materialDefinitionRepository;
    }

    public async Task<bool> Handle(UpdateWorkCenterOutputCommand request, CancellationToken cancellationToken)
    {
        var workCenter = await GetWorkCenterByAbsolutePath(request.AbsolutePath);
        var workCenterOutput = await _workCenterOutputRepository.GetAsync(workCenter.Id, request.MaterialDefinitionId) ?? throw new ResourceNotFoundException(nameof(WorkCenterOutput), $"'{request.AbsolutePath}', '{request.MaterialDefinitionId}'");
        var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId) ?? throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialDefinitionId);
        var materialUnit = materialDefinition.GetMaterialUnitWithId(request.MaterialUnitId);

        workCenterOutput.Update(request.Output, materialUnit);
        _workCenterOutputRepository.Update(workCenterOutput);

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
