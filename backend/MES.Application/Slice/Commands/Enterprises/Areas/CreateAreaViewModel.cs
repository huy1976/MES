namespace MesMicroservice.Application.Commands.Enterprises.Areas;

[DataContract]
public class CreateAreaViewModel
{
    [DataMember]
    public string AreaId { get; set; }
    [DataMember]
    public string Name { get; set; }

    public CreateAreaViewModel(string areaId, string name)
    {
        AreaId = areaId;
        Name = name;
    }
}
