namespace MesMicroservice.Application.Commands.MaterialDefinitions.MaterialUnits;

public class DeleteMaterialUnitCommandHandler : IRequestHandler<DeleteMaterialUnitCommand, bool>
{
    private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

    public DeleteMaterialUnitCommandHandler(IMaterialDefinitionRepository materialDefinitionRepository)
    {
        _materialDefinitionRepository = materialDefinitionRepository;
    }

    public async Task<bool> Handle(DeleteMaterialUnitCommand request, CancellationToken cancellationToken)
    {
        var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId) ?? throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialDefinitionId);

        await _materialDefinitionRepository.DeleteMaterialUnitAsync(request.UnitId);
        materialDefinition.RemoveMaterialUnit(request.UnitId);
        return await _materialDefinitionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
