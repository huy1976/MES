namespace MesMicroservice.Domain.AggregateModels.SegmentAggregate
{
    public interface IEquipmentSegmentSpecificationRepository : IRepository<EquipmentSegmentSpecification>
    {
        public Task<EquipmentSegmentSpecification> Add(EquipmentSegmentSpecification equipmentSegmentSpecification);
        public Task<EquipmentSegmentSpecification?> GetAsync(string equipmentSegmentSpecificationId);
        public Task<IEnumerable<EquipmentSegmentSpecification>> GetEquipmentSegmentSpecifications();
        public Task<IEnumerable<EquipmentSegmentSpecification>> GetEquipmentSegmentSpecificationsByProcessSegmentIdAsync(string processSegmentId);
        public Task<List<EquipmentSegmentSpecification>> GetListByIdAsync(List<string> equipmentSegmentSpecificationIds);
        public EquipmentSegmentSpecification Update(EquipmentSegmentSpecification equipmentSegmentSpecification);
        public Task<bool> ExistsAsync(string equipmentSegmentSpecificationId);
        public Task DeleteAsync(string equipmentSegmentSpecificationId);
    }
}
