

namespace MesMicroservice.Infrastructure.Repositories.PersonModelRepositories
{
    public class PersonRepository : BaseRepository, IPersonRepository
    {
        public PersonRepository(ApplicationDbContext context) : base(context)
        {

        }
        public async Task<Person> Add(Person person)
        {
            if (person.IsTransient())
            {
                if (await ExistsAsync(person.ResourceId))
                {
                    throw new EntityDuplicationException($"Person with id {person.Id} already exists.");
                }

                return _context.Persons
                    .Add(person)
                    .Entity;
            }
            else
            {
                return person;
            }
        }

        public async Task<Person?> GetAsync(string personId)
        {
            return await _context.Persons
                .Include(x => x.PersonnelClass)
                .Include(x => x.Properties)
                .Include(x => x.PersonnelCapabilities)
                .Include(x => x.PersonWorkOrders)
                .FirstOrDefaultAsync(x => x.ResourceId == personId);
        }

        public async Task<IEnumerable<Person>> GetPeople()
        {
            return await _context.Persons
                .Include(x => x.PersonnelClass)
                .Include(x => x.Properties)
                .Include(x => x.PersonnelCapabilities)
                .ToListAsync();
        }

        public async Task<List<Person>> GetListByIdAsync(List<string> personIds)
        {
            var persons = await _context.Persons
                .Include(x => x.PersonnelClass)
                .Where(x => personIds.Contains(x.ResourceId))
                .ToListAsync();

            var notFoundIds = personIds
                .Where(id => !persons.Exists(pc => pc.ResourceId == id));

            if (notFoundIds.Any())
            {
                throw new EntitiesNotFoundException(nameof(Person), notFoundIds.ToList());
            }

            return persons;
        }

        public Person Update(Person person)
        {
            return _context.Persons
                    .Update(person)
                    .Entity;
        }

        public async Task<bool> ExistsAsync(string personId)
        {
            return await _context.Persons
                .AnyAsync(x => x.ResourceId == personId);
        }

        public async Task DeleteAsync(string personId)
        {
            var person = await _context.Persons
                .FirstOrDefaultAsync(x => x.ResourceId == personId);

            if (person is not null)
            {
                _context.Persons.Remove(person);
            }
        }

        public async Task<List<Person>> GetPersonByClassId(string classId)
        {
            return await _context.Persons
                .Include(x => x.PersonnelClass)
                .Include(x => x.PersonnelCapabilities)
                    .ThenInclude(pc => pc.Properties)
                .Where(x => x.PersonnelClass.ResourceId == classId)
                .ToListAsync();
        }

        public Task<List<Person>> GetAll()
        {
            return _context.Persons
                .AsNoTracking()
                .Include(x => x.Properties)
                .Include(x => x.PersonnelClass)
                    .ThenInclude(pc => pc.Properties)
                .ToListAsync();
        }
    }
}
