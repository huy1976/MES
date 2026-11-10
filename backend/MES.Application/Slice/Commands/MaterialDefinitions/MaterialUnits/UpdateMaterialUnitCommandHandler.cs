namespace MesMicroservice.Application.Commands.MaterialDefinitions.MaterialUnits;

public class UpdateMaterialUnitCommandHandler : IRequestHandler<UpdateMaterialUnitCommand, bool>
{
    private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

    public UpdateMaterialUnitCommandHandler(IMaterialDefinitionRepository materialDefinitionRepository)
    {
        _materialDefinitionRepository = materialDefinitionRepository;
    }

    public async Task<bool> Handle(UpdateMaterialUnitCommand request, CancellationToken cancellationToken)
    {
        var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId) ?? throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialDefinitionId);

        materialDefinition.UpdateMaterialUnit(request.UnitId, request.UnitName, request.ConversionValueToPrimaryUnit);
        return await _materialDefinitionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
