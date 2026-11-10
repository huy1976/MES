namespace MesMicroservice.Domain.AggregateModels.ManufacturingOrderAggregate;

public interface IManufacturingOrderRepository : IRepository<ManufacturingOrder>
{
    public Task<ManufacturingOrder> Add(ManufacturingOrder manufacturingOrder);
    public Task<ManufacturingOrder?> GetAsync(string manufacturingOrderId);
    public Task<List<ManufacturingOrder>> GetAll(); 
    public Task<ManufacturingOrder?> GetByIdAsync(string manufacturingOrderId);
    public Task<ManufacturingOrder> GetById(string moId);
    public Task<IEnumerable<ManufacturingOrder>> GetManufacturingOrders();
    public Task<List<ManufacturingOrder>> GetListByIdAsync(List<string> manufacturingOrderIds);
    public ManufacturingOrder Update(ManufacturingOrder manufacturingOrder);
    public Task UpdateStatusById(string manufacturingOrderId);
    public Task UpdateDatesById(string manufacturingOrderId);
    public Task<bool> ExistsAsync(string manufacturingOrderId);
    public Task DeleteAsync(string manufacturingOrderId);
    public Task<List<ManufacturingOrder>?> GetMOByTime(DateTime from, DateTime to);
}
