
namespace MesMicroservice.Infrastructure.Repositories.PersonModelRepositories
{
    public class PersonnelClassRepository : BaseRepository, IPersonnelClassRepository
    {
        public PersonnelClassRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PersonnelClass> AddAsync(PersonnelClass personnelClass)
        {
            if (personnelClass.IsTransient())
            {
                if (await CheckExistenceAsync(personnelClass.ResourceId))
                {
                    throw new EntityDuplicationException(nameof(PersonnelClass), personnelClass.ResourceId);
                }

                return _context.PersonnelClasses
                    .Add(personnelClass)
                    .Entity;
            }
            else
            {
                return personnelClass;
            }
        }


        public async Task<IEnumerable<PersonnelClass>> GetPersonnelClasses()
        {
            return await _context.PersonnelClasses
                    .Include(x => x.Persons)
                    .Include(x => x.Properties)
                    .ToListAsync();
        }



        public async Task<bool> CheckExistenceAsync(string personnelClassId)
        {
            return await _context.PersonnelClasses
                .AnyAsync(x => x.ResourceId == personnelClassId);
        }

        public Task Delete(string personnelClassId)
        {
            throw new NotImplementedException();
        }

        public Task<PersonnelClass?> GetAsync(string personnelClassId)
        {
            return _context.PersonnelClasses
                .FirstOrDefaultAsync(x => x.ResourceId == personnelClassId);
        }

        public Task<List<PersonnelClass>> GetListByIdsAsync(List<string> personnelClassIds)
        {
            throw new NotImplementedException();
        } 

        public PersonnelClass Update(PersonnelClass personnelClass)
        {
            throw new NotImplementedException();
        }
    }
}
