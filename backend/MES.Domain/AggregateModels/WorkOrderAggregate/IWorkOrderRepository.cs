namespace MesMicroservice.Domain.AggregateModels.WorkOrderAggregate;

public interface IWorkOrderRepository : IRepository<WorkOrder>
{
    public Task<WorkOrder> Add(int manufacturingOrderId, WorkOrder workOrder);
    public Task<WorkOrder?> GetAsync(int manufacturingOrderId, string workOrderId);
    public Task<WorkOrder?> GetByWoIdAsync(string workOrderId);
    public Task<WorkOrder?> GetListBomByWoIdAsync(string workOrderId);
    public Task<int> GetQuantityWorkOrders();
    public Task<List<WorkOrder>> GetManufacturingOrderWorkOrderByIdsAsync(int manufacturingOrderId, List<string> workOrderIds);
    public Task<List<WorkOrder>> GetListByIds(List<(string ManufacturingOrderId, string WorkOrderId)> ids);
    public Task<List<WorkOrder>> GetListByStatus(EStatus workOrderStatus);
    public WorkOrder Update(WorkOrder workOrder);
    Task<List<WorkOrder>> GetByWorkOrderCodesAsync(List<string> workOrderCodes);
    public Task<bool> ExistsAsync(int manufacturingOrderId, string workOrderId);
    public Task DeleteAsync(int manufacturingOrderId, string workOrderId);
    public Task<List<WorkOrder>> GetByIds(List<int> ids);
    public Task<List<WorkOrder>> GetByWoIds(List<string> ids);
    public Task<WorkOrder> GetById(int id);
    public Task<bool> ExistsAsync(string workOrderId);
    public Task<List<WorkOrder>> GetByAssignedEquipmentId(string equipmentId);
    public Task<List<WorkOrder>> GetByAssignedPersonId(string personId);
    public Task<IEnumerable<WorkOrder>> GetAll();
    public Task<List<WorkOrder>?> GetWOByTime(DateTime from, DateTime to);
    public Task<List<WorkOrder>> GetAlAsync();
    Task AddAsync(WorkOrder workOrder);
}
