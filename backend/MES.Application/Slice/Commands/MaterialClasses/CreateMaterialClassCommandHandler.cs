namespace MesMicroservice.Application.Commands.MaterialClasses;

public class CreateMaterialClassCommandHandler : IRequestHandler<CreateMaterialClassCommand, bool>
{
    private readonly IMaterialClassRepository _materialClassRepository;

    public CreateMaterialClassCommandHandler(IMaterialClassRepository materialClassRepository)
    {
        _materialClassRepository = materialClassRepository;
    }

    public async Task<bool> Handle(CreateMaterialClassCommand request, CancellationToken cancellationToken)
    {

        var properties = new List<MaterialClassProperty>();
        if (request.Properties.Count() != 0)
        {
            properties = request.Properties.ConvertAll(x => new MaterialClassProperty(
                x.PropertyId,
                x.Description,
                new Value(x.ValueString, x.ValueType),
                x.ValueUnitOfMeasure,
                x.Name));
        }

        var newMaterialClass = new MaterialClass(materialClassId: request.MaterialClassId, 
                                                 name: request.Name,
                                                 description: request.Description,
                                                 assemblyRelationship: request.AssemblyRelationship ?? EAssemblyRelationship.None,
                                                 assemblyType: request.AssemblyType ?? EAssemblyType.None,
                                                 properties: properties);

        await _materialClassRepository.AddAsync(newMaterialClass);

        return await _materialClassRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
