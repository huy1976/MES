namespace MesMicroservice.Application.Commands.MaterialClasses;

public class UpdateMaterialClassCommandHandler : IRequestHandler<UpdateMaterialClassCommand, bool>
{
    private readonly IMaterialClassRepository _materialClassRepository;

    public UpdateMaterialClassCommandHandler(IMaterialClassRepository materialClassRepository)
    {
        _materialClassRepository = materialClassRepository;
    }

    public async Task<bool> Handle(UpdateMaterialClassCommand request, CancellationToken cancellationToken)
    {
        var materialClass = await _materialClassRepository.GetAsync(request.MaterialClassId) ?? throw new ResourceNotFoundException(nameof(MaterialClass), request.MaterialClassId);

        materialClass.Update(request.Name);

        _materialClassRepository.Update(materialClass);
        return await _materialClassRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
