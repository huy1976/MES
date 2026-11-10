namespace MesMicroservice.Application.Commands.MaterialDefinitions.Operations;

[DataContract]
public class UpdateOperationViewModel
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public List<string> PrerequisiteOperation { get; set; }

    public UpdateOperationViewModel(string name, List<string> prerequisiteOperation)
    {
        Name = name;
        PrerequisiteOperation = prerequisiteOperation;
    }
}
