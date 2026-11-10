namespace MesMicroservice.Domain.AggregateModels;

public interface IResourceRepository : IRepository<Resource>
{
    public Task<Resource?> GetAsync(string resourceId);
}
