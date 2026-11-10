namespace MesMicroservice.Application.Commands.MaterialDefinitions;

public class CreateMaterialDefinitionCommandHandler : IRequestHandler<CreateMaterialDefinitionCommand, bool>
{
    private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
    private readonly IMaterialClassRepository _materialClassRepository;

    public CreateMaterialDefinitionCommandHandler(IMaterialDefinitionRepository materialDefinitionRepository, IMaterialClassRepository materialClassRepository)
    {
        _materialDefinitionRepository = materialDefinitionRepository;
        _materialClassRepository = materialClassRepository;
    }

    public async Task<bool> Handle(CreateMaterialDefinitionCommand request, CancellationToken cancellationToken)
    {
        var properties = request.Properties.Select(x => new MaterialDefinitionProperty(
            x.PropertyId,
            x.Description,
            new Value(x.ValueString, x.ValueType),
            x.ValueUnitOfMeasure,
            x.Name))
            .ToList();

        var materialClass = await _materialClassRepository.GetAsync(request.MaterialClass) ?? throw new ResourceNotFoundException(nameof(MaterialClass), request.MaterialClass);
        
        var materialDefinition = new MaterialDefinition(materialDefinitionId: request.MaterialDefinitionId, 
                                                        name: request.Name, 
                                                        primaryUnit: request.PrimaryUnit,
                                                        assemblyType: request.AssemblyType ?? EAssemblyType.None,
                                                        assemblyRelationship: request.AssemblyRelationship ?? EAssemblyRelationship.None,
                                                        properties: properties, 
                                                        materialClass: materialClass);

        await _materialDefinitionRepository.Add(materialDefinition);

        return await _materialDefinitionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
