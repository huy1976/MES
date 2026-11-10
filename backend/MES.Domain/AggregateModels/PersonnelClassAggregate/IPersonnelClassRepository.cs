namespace MesMicroservice.Domain.AggregateModels.PersonnelClassAggregate
{
    public interface IPersonnelClassRepository : IRepository<PersonnelClass>
    {
        public Task<PersonnelClass> AddAsync(PersonnelClass personnelClass);
        public Task<PersonnelClass?> GetAsync(string personnelClassId);

        public Task<IEnumerable<PersonnelClass>> GetPersonnelClasses();
        public PersonnelClass Update(PersonnelClass personnelClass);
        public Task<List<PersonnelClass>> GetListByIdsAsync(List<string> personnelClassIds);
        public Task<bool> CheckExistenceAsync(string personnelClassId);
        public Task Delete(string personnelClassId);
    }
}
