namespace MesMicroservice.Domain.AggregateModels.WorkOrderAggregate
{
    public interface IWorkOrderErrorRecord : IRepository<WorkOrderErrorRecord>
    {
        Task<List<WorkOrderErrorRecord>> GetByWorkOrderId(string Id);
        Task<List<WorkOrderErrorRecord>> GetAllAsync();
        Task<WorkOrderErrorRecord> Add(WorkOrderErrorRecord record);

    }
}
