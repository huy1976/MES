
namespace MesMicroservice.Infrastructure.Repositories.PersonModelRepositories
{
    public class PersonnelCapabilityRepository : BaseRepository, IPersonnelCapabilityRepository
    {
        public PersonnelCapabilityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PersonnelCapability> Add(PersonnelCapability personnelCapability)
        {
            if (personnelCapability.IsTransient())
            {
                if (await ExistsAsync(personnelCapability.ResourceId))
                {
                    throw new EntityDuplicationException($"PersonnelCapability with id {personnelCapability.Id} already exists.");
                }

                return _context.PersonnelCapabilities
                    .Add(personnelCapability)
                    .Entity;
            }
            else
            {
                return personnelCapability;
            }
        }

        public async Task<PersonnelCapability?> GetAsync(string personnelCapabilityId)
        {
            return await _context.PersonnelCapabilities
                .FirstOrDefaultAsync(x => x.ResourceId == personnelCapabilityId);
        }

        public async Task<List<PersonnelCapability>> GetAllAsync()
        {
            return await _context.PersonnelCapabilities
                .AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<PersonnelCapability>> GetPersonnelCapabilities()
        {
            return await _context.PersonnelCapabilities
                .Include(x => x.Properties)
                .Include(x => x.Person)
                .ToListAsync();
        }

        public PersonnelCapability Update(PersonnelCapability personnelCapability)
        {
            return _context.PersonnelCapabilities.Update(personnelCapability).Entity;
        }

        public async Task<bool> ExistsAsync(string personnelCapabilityId)
        {
            return await _context.PersonnelCapabilities
                .AnyAsync(x => x.ResourceId == personnelCapabilityId);
        }

        public async Task DeleteAsync(string personnelCapabilityId)
        {
            var entity = await GetAsync(personnelCapabilityId);
            if (entity != null)
            {
                _context.PersonnelCapabilities.Remove(entity);
            }
        }

    }
}
