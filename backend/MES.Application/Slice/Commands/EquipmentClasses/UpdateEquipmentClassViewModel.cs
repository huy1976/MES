namespace MesMicroservice.Application.Commands.EquipmentClasses;

[DataContract]
public class UpdateEquipmentClassViewModel
{
    [DataMember]
    public string Name { get; set; }

    public UpdateEquipmentClassViewModel(string name)
    {
        Name = name;
    }
}
