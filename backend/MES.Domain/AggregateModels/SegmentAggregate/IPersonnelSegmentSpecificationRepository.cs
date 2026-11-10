namespace MesMicroservice.Domain.AggregateModels.SegmentAggregate
{
    public interface IPersonnelSegmentSpecificationRepository : IRepository<PersonnelSegmentSpecification>
    {
        public Task<PersonnelSegmentSpecification> Add(PersonnelSegmentSpecification personnelSegmentSpecification);
        public Task<PersonnelSegmentSpecification?> GetAsync(string personnelSegmentSpecificationId);
        public Task<IEnumerable<PersonnelSegmentSpecification>> GetPersonnelSegmentSpecifications();
        public Task<IEnumerable<PersonnelSegmentSpecification>> GetPersonnelSegmentSpecificationsByProcessSegmentIdAsync(string processSegmentId);
        public Task<List<PersonnelSegmentSpecification>> GetListByIdAsync(List<string> personnelSegmentSpecificationIds);
        public PersonnelSegmentSpecification Update(PersonnelSegmentSpecification personnelSegmentSpecification);
        public Task<bool> ExistsAsync(string personnelSegmentSpecificationId);
        public Task DeleteAsync(string personnelSegmentSpecificationId);
    }
}
