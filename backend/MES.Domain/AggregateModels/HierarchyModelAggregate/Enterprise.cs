namespace MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;
public class Enterprise : HierarchyModel, IAggregateRoot
{
    public List<Site> Sites { get; private set; }

    public Enterprise() : base()
    {
        Sites = new List<Site>();
    }
    public Enterprise(string enterpriseId, string name) : base(enterpriseId, name)
    {
        Sites = new List<Site>();
    }

    public void Update(string enterpriseId, string name)
    {
        HierarchyModelId = enterpriseId;
        Name = name;
    }

    public void AddSite(Site site)
    {
        if (Sites.Any(x => x.HierarchyModelId == site.HierarchyModelId))
        {
            throw new ChildEntityDuplicationException(site.HierarchyModelId, site, this.HierarchyModelId, this);
        }

        Sites.Add(site);
    }

    public void RemoveSite(string siteId)
    {
        var site = Sites.Find(x => x.HierarchyModelId == siteId)
            ?? throw new ChildEntityNotFoundException(siteId, typeof(Site), this.HierarchyModelId, this);

        Sites.Remove(site);
    }
}
