namespace MesMicroservice.Application.Queries.Enterprises;

public class AreaViewModel
{
    public string AreaId { get; set; }
    public string Name { get; set; }
    public string AbsolutePath { get; set; }
    public List<WorkCenterViewModel> WorkCenters { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public AreaViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public AreaViewModel(string areaId, string name, string absolutePath, List<WorkCenterViewModel> workCenters)
    {
        AreaId = areaId;
        Name = name;
        AbsolutePath = absolutePath;
        WorkCenters = workCenters;
    }
}
