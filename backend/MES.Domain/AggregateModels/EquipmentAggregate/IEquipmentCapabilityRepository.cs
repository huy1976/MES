namespace MesMicroservice.Domain.AggregateModels.EquipmentCapabilityAggregate
{
    public interface IEquipmentCapabilityRepository : IRepository<EquipmentCapability>
    {
        public Task<EquipmentCapability> Add(EquipmentCapability equipmentCapability);
        public Task<EquipmentCapability?> GetAsync(string equipmentCapabilityId);
        public Task<IEnumerable<EquipmentCapability>> GetEquipmentCapabilities();
        public EquipmentCapability Update(EquipmentCapability equipmentCapability);
        public Task<bool> ExistsAsync(string equipmentCapabilityId);
        public Task DeleteAsync(string equipmentCapabilityId);
    }
}
