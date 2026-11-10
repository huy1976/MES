namespace MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;
public class Site : HierarchyModel, IAggregateRoot
{
    public List<Area> Areas { get; private set; }

    public Site() : base()
    {
        Areas = new List<Area>();
    }
    public Site(string siteId, string name) : base(siteId, name)
    {
        Areas = new List<Area>();
    }

    public void Update(string siteId, string name)
    {
        HierarchyModelId = siteId;
        Name = name;
    }

    public void AddArea(Area area)
    {
        if (Areas.Any(x => x.HierarchyModelId == area.HierarchyModelId))
        {
            throw new ChildEntityDuplicationException(area.HierarchyModelId, area, this.HierarchyModelId, this);
        }

        Areas.Add(area);
    }

    public void RemoveArea(string areaId)
    {
        var area = Areas.Find(x => x.HierarchyModelId == areaId)
            ?? throw new ChildEntityNotFoundException(areaId, typeof(Area), this.HierarchyModelId, this);
        
        Areas.Remove(area);
    }
}
