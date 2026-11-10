namespace MesMicroservice.Domain.AggregateModels.SegmentAggregate
{
    public interface IMaterialSegmentSpecificationRepository : IRepository<MaterialSegmentSpecification>
    {
        public Task<MaterialSegmentSpecification> Add(MaterialSegmentSpecification materialSegmentSpecification);
        public Task<MaterialSegmentSpecification?> GetAsync(string materialSegmentSpecificationId);
        public Task<IEnumerable<MaterialSegmentSpecification>> GetMaterialSegmentSpecifications();
        public Task<IEnumerable<MaterialSegmentSpecification>> GetMaterialSegmentSpecificationsByProcessSegmentIdAsync(string processSegmentId);
        public Task<List<MaterialSegmentSpecification>> GetListByIdAsync(List<string> materialSegmentSpecificationIds);
        public MaterialSegmentSpecification Update(MaterialSegmentSpecification materialSegmentSpecification);
        public Task<bool> ExistsAsync(string materialSegmentSpecificationId);
        public Task DeleteAsync(string materialSegmentSpecificationId);
    }
}
