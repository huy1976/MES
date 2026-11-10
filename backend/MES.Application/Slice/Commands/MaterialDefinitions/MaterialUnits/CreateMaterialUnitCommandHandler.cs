namespace MesMicroservice.Application.Commands.MaterialDefinitions.MaterialUnits;

public class CreateMaterialUnitCommandHandler : IRequestHandler<CreateMaterialUnitCommand, bool>
{
    private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

    public CreateMaterialUnitCommandHandler(IMaterialDefinitionRepository materialDefinitionRepository)
    {
        _materialDefinitionRepository = materialDefinitionRepository;
    }

    public async Task<bool> Handle(CreateMaterialUnitCommand request, CancellationToken cancellationToken)
    {
        var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId) ?? throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialDefinitionId);

        materialDefinition.AddMaterialUnit(request.UnitId, request.UnitName, request.ConversionValueToPrimaryUnit);
        return await _materialDefinitionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
