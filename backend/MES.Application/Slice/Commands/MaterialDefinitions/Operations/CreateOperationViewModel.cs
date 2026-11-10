namespace MesMicroservice.Application.Commands.MaterialDefinitions.Operations;

[DataContract]
public class CreateOperationViewModel
{
    [DataMember]
    public string OperationId { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public List<string> PrerequisiteOperation { get; set; }

    public CreateOperationViewModel(string operationId, string name, List<string> prerequisiteOperation)
    {
        OperationId = operationId;
        Name = name;
        PrerequisiteOperation = prerequisiteOperation;
    }
}
