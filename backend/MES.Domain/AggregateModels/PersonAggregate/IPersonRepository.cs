namespace MesMicroservice.Domain.AggregateModels.PersonAggregate
{
    public interface IPersonRepository : IRepository<Person>
    {
        public Task<Person> Add(Person person);
        public Task<Person?> GetAsync(string personId);
        public Task<List<Person>> GetPersonByClassId(string classId);
        public Task<IEnumerable<Person>> GetPeople();
        public Task<List<Person>> GetListByIdAsync(List<string> personIds);
        public Person Update(Person person);
        public Task<bool> ExistsAsync(string personId);
        public Task DeleteAsync(string personId);
        public Task<List<Person>> GetAll();
    }
}
