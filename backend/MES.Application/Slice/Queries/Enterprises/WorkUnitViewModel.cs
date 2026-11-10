namespace MesMicroservice.Application.Queries.Enterprises;

public class WorkUnitViewModel
{
    public string WorkUnitId { get; set; }
    public string Name { get; set; }
    public string AbsolutePath { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public WorkUnitViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public WorkUnitViewModel(string workUnitId, string name, string absolutePath)
    {
        WorkUnitId = workUnitId;
        Name = name;
        AbsolutePath = absolutePath;
    }
}
