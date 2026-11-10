namespace MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

public interface ISiteRepository : IRepository<Site>
{
    public Task<IEnumerable<Site>> GetSites();
}
