namespace MesMicroservice.Application.Commands.Enterprises;
[DataContract]
public class UpdateEnterpriseViewModel
{
    [DataMember]
    public string EnterpriseId { get; set; }
    [DataMember]
    public string Name { get; set; }

    public UpdateEnterpriseViewModel(string enterpriseId, string name)
    {
        EnterpriseId = enterpriseId;
        Name = name;
    }
}
