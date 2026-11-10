namespace MesMicroservice.Domain.AggregateModels.DowntimeAggregate
{
    public interface IDowntimeRepository : IRepository<Downtime>
    {
        public Downtime Add(Downtime downtime);
    }
}
