namespace MesMicroservice.Application.Commands.Enterprises.WorkCenters;

[DataContract]
public class CreateWorkCenterOutputViewModel
{
    [DataMember]
    public string MaterialDefinitionId { get; set; }
    [DataMember]
    public decimal Output { get; set; }
    [DataMember]
    public string MaterialUnitId { get; set; }

    public CreateWorkCenterOutputViewModel(string materialDefinitionId, decimal output, string materialUnitId)
    {
        MaterialDefinitionId = materialDefinitionId;
        Output = output;
        MaterialUnitId = materialUnitId;
    }
}
