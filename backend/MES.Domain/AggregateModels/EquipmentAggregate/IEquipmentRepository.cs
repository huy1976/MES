namespace MesMicroservice.Domain.AggregateModels.EquipmentAggregate;
public interface IEquipmentRepository: IRepository<Equipment>
{
    public Task<Equipment> Add(Equipment equipment);
    public Task<Equipment?> GetAsync(string equipmentId);
    public Task<IEnumerable<Equipment>> GetEquipments();
    public Task<List<Equipment>> GetAllAsync();
    public Equipment Update(Equipment equipment);
    public Task<bool> ExistsAsync(string equipmentId);
    public Task DeleteAsync(string equipmentId);
    public Task<WorkCenter> GetWorkCenterByEquipmentId(string workCenterId);
    public Task<List<Equipment>> GetAll();
}
