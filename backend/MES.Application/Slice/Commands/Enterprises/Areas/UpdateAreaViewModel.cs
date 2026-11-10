namespace MesMicroservice.Application.Commands.Enterprises.Areas;
[DataContract]
public class UpdateAreaViewModel
{
    [DataMember]
    public string AreaId { get; set; }
    [DataMember]
    public string Name { get; set; }

    public UpdateAreaViewModel(string areaId, string name)
    {
        AreaId = areaId;
        Name = name;
    }
}
