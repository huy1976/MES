namespace MesMicroservice.Application.Commands.Enterprises;

[DataContract]
public class CreateEnterpriseCommand : IRequest<bool>
{
    [DataMember]
    public string EnterpriseId { get; set; }
    [DataMember]
    public string Name { get; set; }

    public CreateEnterpriseCommand(string enterpriseId, string name)
    {
        EnterpriseId = enterpriseId;
        Name = name;
    }
}
