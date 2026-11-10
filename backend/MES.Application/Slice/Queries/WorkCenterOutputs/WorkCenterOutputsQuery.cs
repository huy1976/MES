namespace MesMicroservice.Application.Queries.WorkCenterOutputs;

public class WorkCenterOutputsQuery : IRequest<IEnumerable<WorkCenterOutputViewModel>>
{
    public string AbsolutePath { get; set; }

    public WorkCenterOutputsQuery(string absolutePath)
    {
        AbsolutePath = absolutePath;
    }
}
