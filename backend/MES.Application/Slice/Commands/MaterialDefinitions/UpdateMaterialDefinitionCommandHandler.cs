namespace MesMicroservice.Application.Commands.MaterialDefinitions;

public class UpdateMaterialDefinitionCommandHandler : IRequestHandler<UpdateMaterialDefinitionCommand, bool>
{
    private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
    private readonly IMaterialClassRepository _materialClassRepository;

    public UpdateMaterialDefinitionCommandHandler(IMaterialDefinitionRepository materialDefinitionRepository, IMaterialClassRepository materialClassRepository)
    {
        _materialDefinitionRepository = materialDefinitionRepository;
        _materialClassRepository = materialClassRepository;
    }

    public async Task<bool> Handle(UpdateMaterialDefinitionCommand request, CancellationToken cancellationToken)
    {
        var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId) ?? throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialDefinitionId);
        var properties = request.Properties.ConvertAll(x => new MaterialDefinitionProperty(
            x.PropertyId,
            x.Description,
            new Value(x.ValueString, x.ValueType),
            x.ValueUnitOfMeasure,
            x.Name));

        var materialClass = await _materialClassRepository.GetAsync(request.MaterialClass) ?? throw new ResourceNotFoundException(nameof(MaterialClass), request.MaterialClass);

        materialDefinition.Update(request.Name, request.PrimaryUnit, properties, materialClass);

        _materialDefinitionRepository.Update(materialDefinition);

        return await _materialDefinitionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
