

namespace MesMicroservice.Application.Commands.MaterialDefinitions.Operations;

public class DeleteOperationCommandHandler : IRequestHandler<DeleteOperationCommand, bool>
{
    private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

    public DeleteOperationCommandHandler(IMaterialDefinitionRepository materialDefinitionRepository)
    {
        _materialDefinitionRepository = materialDefinitionRepository;
    }

    public async Task<bool> Handle(DeleteOperationCommand request, CancellationToken cancellationToken)
    {
        var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId) ?? throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialDefinitionId);

        await _materialDefinitionRepository.DeleteOperationAsync(request.OperationId);
        materialDefinition.RemoveOperationsDefinition(request.OperationId);

        return await _materialDefinitionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
