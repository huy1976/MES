
namespace MesMicroservice.Infrastructure.Repositories.SegmentModelRepositories
{
    public class MaterialSegmentSpecificationRepository : BaseRepository, IMaterialSegmentSpecificationRepository
    {
        public MaterialSegmentSpecificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<MaterialSegmentSpecification> Add(MaterialSegmentSpecification materialSegmentSpecification)
        {
            if (materialSegmentSpecification.IsTransient())
            {
                if (await ExistsAsync(materialSegmentSpecification.ResourceId))
                {
                    throw new EntityDuplicationException(nameof(MaterialDefinition), materialSegmentSpecification.ResourceId);
                }

                return _context.MaterialSegmentSpecifications
                    .Add(materialSegmentSpecification)
                    .Entity;
            }
            else
            {
                return materialSegmentSpecification;
            }
        }

        public async Task DeleteAsync(string materialSegmentSpecificationId)
        {
            var materialSegmentSpecification = await _context.MaterialSegmentSpecifications
                .FirstOrDefaultAsync(x => x.ResourceId == materialSegmentSpecificationId);

            if (materialSegmentSpecification is not null)
            {
                _context.MaterialSegmentSpecifications.Remove(materialSegmentSpecification);
            }
        }

        public async Task<bool> ExistsAsync(string materialSegmentSpecificationId)
        {
            return await _context.MaterialSegmentSpecifications
                .AnyAsync(x => x.ResourceId == materialSegmentSpecificationId);
        }

        public async Task<MaterialSegmentSpecification?> GetAsync(string materialSegmentSpecificationId)
        {
            return await _context.MaterialSegmentSpecifications
                .Include(x => x.Properties)
                .Include(x => x.ProcessSegment)
                .Include(x => x.MaterialDefinition)
                .FirstOrDefaultAsync(x => x.ResourceId == materialSegmentSpecificationId);
        }

        public async Task<IEnumerable<MaterialSegmentSpecification>> GetMaterialSegmentSpecifications()
        {
            return await _context.MaterialSegmentSpecifications
                .Include(x => x.Properties)
                .Include(x => x.ProcessSegment)
                .Include(x => x.MaterialDefinition)
                    .ThenInclude(x => x.MaterialClass)
                .ToListAsync();
        }

        public async Task<IEnumerable<MaterialSegmentSpecification>> GetMaterialSegmentSpecificationsByProcessSegmentIdAsync(string processSegmentId)
        {
            return await _context.MaterialSegmentSpecifications
            .Include(x => x.Properties)
            .Include(x => x.ProcessSegment)
            .Include(x => x.MaterialDefinition)
                .ThenInclude(x => x.MaterialClass)
            .Where(x => x.ProcessSegment.ResourceId == processSegmentId)
            .ToArrayAsync();
        }


        public async Task<List<MaterialSegmentSpecification>> GetListByIdAsync(List<string> materialSegmentSpecificationIds)
        {
            var materialSegmentSpecifications = await _context.MaterialSegmentSpecifications
                .Include(x => x.Properties)
                .Include(x => x.ProcessSegment)
                .Include(x => x.MaterialDefinition)
                .Where(x => materialSegmentSpecificationIds.Contains(x.ResourceId))
                .ToListAsync();

            var notFoundIds = materialSegmentSpecificationIds
                .Where(id => !materialSegmentSpecifications.Exists(pc => pc.ResourceId == id));

            if (notFoundIds.Any())
            {
                throw new EntitiesNotFoundException(nameof(MaterialDefinition), notFoundIds.ToList());
            }

            return materialSegmentSpecifications;
        }

        public MaterialSegmentSpecification Update(MaterialSegmentSpecification materialSegmentSpecification)
        {
            return _context.MaterialSegmentSpecifications
                    .Update(materialSegmentSpecification)
                    .Entity;
        }

 
    }
}
