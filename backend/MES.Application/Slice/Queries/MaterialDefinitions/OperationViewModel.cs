namespace MesMicroservice.Application.Queries.MaterialDefinitions;

public class OperationViewModel
{
    public string OperationId { get; set; }
    public string Name { get;  set; }
    public List<string> PrerequisiteOperation { get; set; }

    public OperationViewModel(string operationId, string name, List<string> prerequisiteOperation)
    {
        OperationId = operationId;
        Name = name;
        PrerequisiteOperation = prerequisiteOperation;
    }
}
