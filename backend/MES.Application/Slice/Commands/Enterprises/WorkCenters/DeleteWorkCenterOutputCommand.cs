namespace MesMicroservice.Application.Commands.Enterprises.WorkCenters;

public class DeleteWorkCenterOutputCommand : IRequest<bool>
{
    public string AbsolutePath { get; set; }
    public string MaterialDefinitionId { get; set; }

    public DeleteWorkCenterOutputCommand(string absolutePath, string materialDefinitionId)
    {
        AbsolutePath = absolutePath;
        MaterialDefinitionId = materialDefinitionId;
    }
}
