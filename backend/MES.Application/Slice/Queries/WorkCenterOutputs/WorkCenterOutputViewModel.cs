namespace MesMicroservice.Application.Queries.WorkCenterOutputs;

public class WorkCenterOutputViewModel
{
    public string WorkCenter { get; set; }
    public string MaterialDefinition { get; set; }
    public decimal Output { get; set; }
    public string Unit { get; set; }

    public WorkCenterOutputViewModel(string workCenter, string materialDefinition, decimal output, string unit)
    {
        WorkCenter = workCenter;
        MaterialDefinition = materialDefinition;
        Output = output;
        Unit = unit;
    }
}
