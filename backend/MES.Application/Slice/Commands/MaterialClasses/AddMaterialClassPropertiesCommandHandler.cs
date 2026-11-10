
namespace MesMicroservice.Application.Commands.MaterialClasses;

public class AddMaterialClassPropertiesCommandHandler : IRequestHandler<AddMaterialClassPropertiesCommand, bool>
{
    private readonly IMaterialClassRepository _materialClassRepository;

    public AddMaterialClassPropertiesCommandHandler(IMaterialClassRepository materialClassRepository)
    {
        _materialClassRepository = materialClassRepository;
    }

    public async Task<bool> Handle(AddMaterialClassPropertiesCommand request, CancellationToken cancellationToken)
    {
        var materialclass = await _materialClassRepository.GetAsync(request.MaterialClassId);

        var properties = request.Properties.Select(x => new MaterialClassProperty(
            x.PropertyId,
            x.Description,
            new Value(x.ValueString, x.ValueType),
            x.ValueUnitOfMeasure,
            x.Name))
            .ToList();

        if (materialclass is null)
        {
            throw new ResourceNotFoundException(nameof(MaterialClass), request.MaterialClassId);
        }

        materialclass.AddProperties(properties);

        return await _materialClassRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
