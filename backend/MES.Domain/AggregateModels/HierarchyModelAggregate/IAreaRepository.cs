namespace MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

public interface IAreaRepository 
{
    public Task<IEnumerable<Area>> GetAreas();
}
