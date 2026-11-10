namespace MesMicroservice.Application.Commands.MaterialDefinitions.Operations;

public class UpdateOperationCommand : IRequest<bool>
{
    public string MaterialDefinitionId { get; set; }
    public string OperationId { get; set; }
    public string Name { get; set; }
    public List<string> PrerequisiteOperation { get; set; }

    public UpdateOperationCommand(string materialDefinitionId, string operationId, string name, List<string> prerequisiteOperation)
    {
        MaterialDefinitionId = materialDefinitionId;
        OperationId = operationId;
        Name = name;
        PrerequisiteOperation = prerequisiteOperation;
    }
}
