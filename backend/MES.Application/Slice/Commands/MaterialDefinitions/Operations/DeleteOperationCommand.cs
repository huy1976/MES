namespace MesMicroservice.Application.Commands.MaterialDefinitions.Operations;

public class DeleteOperationCommand : IRequest<bool>
{
    public string MaterialDefinitionId { get; set; }
    public string OperationId { get; set; }

    public DeleteOperationCommand(string materialDefinitionId, string operationId)
    {
        MaterialDefinitionId = materialDefinitionId;
        OperationId = operationId;
    }
}
