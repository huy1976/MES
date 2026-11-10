namespace MesMicroservice.Application.Queries.Enterprises;

public class EnterpriseViewModel
{
    public string EnterpriseId { get; set; }
    public string Name { get; set; }
    public string AbsolutePath { get; set; }
    public List<SiteViewModel> Sites { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public EnterpriseViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public EnterpriseViewModel(string enterpriseId, string name, string absolutePath, List<SiteViewModel> sites)
    {
        EnterpriseId = enterpriseId;
        Name = name;
        AbsolutePath = absolutePath;
        Sites = sites;
    }
}

    
