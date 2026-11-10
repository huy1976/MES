namespace MesMicroservice.Application.Commands;

public class UpdateResourceClassViewModel
{
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public List<SavePropertyViewModel> Properties { get; set; }

    public UpdateResourceClassViewModel(string description, List<SavePropertyViewModel> properties)
    {
        Description = description;
        Properties = properties;
    }
}
