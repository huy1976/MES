namespace MesMicroservice.Domain.AggregateModels.EquipmentClassAggregate;
public interface IEquipmentClassRepository: IRepository<EquipmentClass>
{
    public Task<EquipmentClass> AddAsync(EquipmentClass equipmentClass);
    public Task<EquipmentClass?> GetAsync(string equipmentClassId);

    public Task<IEnumerable<EquipmentClass>> GetEquipmentClasses();
    public EquipmentClass Update(EquipmentClass equipmentClass);
    public Task<List<EquipmentClass>> GetListByIdsAsync(List<string> equipmentClassIds);
    public Task<bool> CheckExistenceAsync(string equipmentClassId);
    public Task Delete(string equipmentClassId);
}
