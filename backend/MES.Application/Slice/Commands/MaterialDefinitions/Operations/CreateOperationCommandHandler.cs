namespace MesMicroservice.Application.Commands.MaterialDefinitions.Operations;

public class CreateOperationCommandHandler : IRequestHandler<CreateOperationCommand, bool>
{
    private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

    public CreateOperationCommandHandler(IMaterialDefinitionRepository materialDefinitionRepository)
    {
        _materialDefinitionRepository = materialDefinitionRepository;
    }

    public async Task<bool> Handle(CreateOperationCommand request, CancellationToken cancellationToken)
    {
        var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId) ?? throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialDefinitionId);

        materialDefinition.AddOperationsDefinition(request.OperationId, request.Name, request.PrerequisiteOperation);
        return await _materialDefinitionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
