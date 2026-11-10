
namespace MesMicroservice.Infrastructure.Repositories.SegmentModelRepositories
{
    public class PersonnelSegmentSpecificationRepository : BaseRepository, IPersonnelSegmentSpecificationRepository
    {
        public PersonnelSegmentSpecificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PersonnelSegmentSpecification> Add(PersonnelSegmentSpecification personnelSegmentSpecification)
        {
            if (personnelSegmentSpecification.IsTransient())
            {
                if (await ExistsAsync(personnelSegmentSpecification.ResourceId))
                {
                    throw new EntityDuplicationException(nameof(PersonnelClass), personnelSegmentSpecification.ResourceId);
                }

                return _context.PersonnelSegmentSpecifications
                    .Add(personnelSegmentSpecification)
                    .Entity;
            }
            else
            {
                return personnelSegmentSpecification;
            }
        }

        public async Task DeleteAsync(string personnelSegmentSpecificationId)
        {
            var personnelSegmentSpecification = await _context.PersonnelSegmentSpecifications
                .FirstOrDefaultAsync(x => x.ResourceId == personnelSegmentSpecificationId);

            if (personnelSegmentSpecification is not null)
            {
                _context.PersonnelSegmentSpecifications.Remove(personnelSegmentSpecification);
            }
        }

        public async Task<bool> ExistsAsync(string personnelSegmentSpecificationId)
        {
            return await _context.PersonnelSegmentSpecifications
                .AnyAsync(x => x.ResourceId == personnelSegmentSpecificationId);
        }

        public async Task<PersonnelSegmentSpecification?> GetAsync(string personnelSegmentSpecificationId)
        {
            return await _context.PersonnelSegmentSpecifications
                .Include(x => x.Properties)
                .Include(x => x.ProcessSegment)
                .Include(x => x.PersonnelClass)
                .FirstOrDefaultAsync(x => x.ResourceId == personnelSegmentSpecificationId);
        }


        public async Task<IEnumerable<PersonnelSegmentSpecification>> GetPersonnelSegmentSpecifications()
        {
            return await _context.PersonnelSegmentSpecifications
                .Include(x => x.Properties)
                .Include(x => x.ProcessSegment)
                .Include(x => x.PersonnelClass)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonnelSegmentSpecification>> GetPersonnelSegmentSpecificationsByProcessSegmentIdAsync(string processSegmentId)
        {
            return await _context.PersonnelSegmentSpecifications
            .Include(x => x.Properties)
            .Include(x => x.ProcessSegment)
            .Include(x => x.PersonnelClass)
            .Where(x => x.ProcessSegment.ResourceId == processSegmentId)
            .ToListAsync();
        }

        public async Task<List<PersonnelSegmentSpecification>> GetListByIdAsync(List<string> personnelSegmentSpecificationIds)
        {
            var personnelSegmentSpecifications = await _context.PersonnelSegmentSpecifications
                .Include(x => x.Properties)
                .Include(x => x.ProcessSegment)
                .Include(x => x.PersonnelClass)
                .Where(x => personnelSegmentSpecificationIds.Contains(x.ResourceId))
                .ToListAsync();

            var notFoundIds = personnelSegmentSpecificationIds
                .Where(id => !personnelSegmentSpecifications.Exists(pc => pc.ResourceId == id));

            if (notFoundIds.Any())
            {
                throw new EntitiesNotFoundException(nameof(PersonnelClass), notFoundIds.ToList());
            }

            return personnelSegmentSpecifications;
        }
        public PersonnelSegmentSpecification Update(PersonnelSegmentSpecification personnelSegmentSpecification)
        {
            return _context.PersonnelSegmentSpecifications
                    .Update(personnelSegmentSpecification)
                    .Entity;
        }

    }

}
