namespace MesMicroservice.Domain.AggregateModels.PersonAggregate
{
    public interface IPersonnelCapabilityRepository : IRepository<PersonnelCapability>
    {
        public Task<PersonnelCapability> Add(PersonnelCapability personnelCapability);
        public Task<PersonnelCapability?> GetAsync(string personnelCapabilityId);
        public Task<List<PersonnelCapability>> GetAllAsync();

        public Task<IEnumerable<PersonnelCapability>> GetPersonnelCapabilities();
        public PersonnelCapability Update(PersonnelCapability personnelCapability);
        public Task<bool> ExistsAsync(string personnelCapabilityId);
        public Task DeleteAsync(string personnelCapabilityId);
    }
}
