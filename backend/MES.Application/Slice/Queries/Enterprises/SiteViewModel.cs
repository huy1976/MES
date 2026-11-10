namespace MesMicroservice.Application.Queries.Enterprises;

public class SiteViewModel
{
    public string SiteId { get; set; }
    public string Name { get; set; }
    public string AbsolutePath { get; set; }
    public List<AreaViewModel> Areas { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SiteViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public SiteViewModel(string siteId, string name, string absolutePath, List<AreaViewModel> areas)
    {
        SiteId = siteId;
        Name = name;
        AbsolutePath = absolutePath;
        Areas = areas;
    }
}
