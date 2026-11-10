namespace MesMicroservice.Application.Commands.Enterprises.WorkCenters;

public class UpdateWorkCenterOutputCommand : IRequest<bool>
{
    public string AbsolutePath { get; set; }
    public string MaterialDefinitionId { get; set; }
    public decimal Output { get; set; }
    public string MaterialUnitId { get; set; }

    public UpdateWorkCenterOutputCommand(string absolutePath, string materialDefinitionId, decimal output, string materialUnitId)
    {
        AbsolutePath = absolutePath;
        MaterialDefinitionId = materialDefinitionId;
        Output = output;
        MaterialUnitId = materialUnitId;
    }
}
