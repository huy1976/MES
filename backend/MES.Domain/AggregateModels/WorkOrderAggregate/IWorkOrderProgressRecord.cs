    namespace MesMicroservice.Domain.AggregateModels.WorkOrderAggregate
{
    public interface IWorkOrderProgressRecord : IRepository<WorkOrderProgressRecord>
    {
        Task<List<WorkOrderProgressRecord>> GetByWorkOrderId(string Id);
        Task<WorkOrderProgressRecord> Add(WorkOrderProgressRecord record);
    }
}
