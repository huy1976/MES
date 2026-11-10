namespace MesMicroservice.Application.Commands.Enterprises.WorkCenters;

[DataContract]
public class UpdateWorkCenterOutputViewModel
{
    [DataMember]
    public decimal Output { get; set; }
    [DataMember]
    public string MaterialUnitId { get; set; }

    public UpdateWorkCenterOutputViewModel(decimal output, string materialUnitId)
    {
        Output = output;
        MaterialUnitId = materialUnitId;
    }
}
