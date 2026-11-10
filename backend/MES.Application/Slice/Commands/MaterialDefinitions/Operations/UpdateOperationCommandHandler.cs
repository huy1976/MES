namespace MesMicroservice.Application.Commands.MaterialDefinitions.Operations;

public class UpdateOperationCommandHandler : IRequestHandler<UpdateOperationCommand, bool>
{
    private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

    public UpdateOperationCommandHandler(IMaterialDefinitionRepository materialDefinitionRepository)
    {
        _materialDefinitionRepository = materialDefinitionRepository;
    }

    public async Task<bool> Handle(UpdateOperationCommand request, CancellationToken cancellationToken)
    {
        var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId) ?? throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialDefinitionId);

        materialDefinition.UpdateOperationsDefinition(request.OperationId, request.Name, request.PrerequisiteOperation);
        return await _materialDefinitionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
