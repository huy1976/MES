
namespace MesMicroservice.Infrastructure.Repositories.QualityStandardRepositories
{
    public class QualityStandardRepository : BaseRepository, IQualityStandardRepository
    {
        public QualityStandardRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<QualityStandard> Add(QualityStandard qualityStandard)
        {
            if (qualityStandard.IsTransient())
            {
                if (await ExistsAsync(qualityStandard.ResourceId))
                {
                    throw new EntityDuplicationException(nameof(MaterialDefinition), qualityStandard.ResourceId);
                }

                return _context.QualityStandards
                    .Add(qualityStandard)
                    .Entity;
            }
            else
            {
                return qualityStandard;
            }
        }

        public async Task DeleteAsync(string qualityStandardId)
        {
            var qualityStandard = await _context.QualityStandards
                .FirstOrDefaultAsync(x => x.ResourceId == qualityStandardId);

            if (qualityStandard is not null)
            {
                _context.QualityStandards.Remove(qualityStandard);
            }
        }

        public async Task<bool> ExistsAsync(string qualityStandardId)
        {
            return await _context.QualityStandards
                .AnyAsync(x => x.ResourceId == qualityStandardId);
        }

        public async Task<QualityStandard?> GetAsync(string qualityStandardId)
        {
            return await _context.QualityStandards
                .Include(x => x.Properties)
                .Include(x => x.MaterialDefinitions)
                .FirstOrDefaultAsync(x => x.ResourceId == qualityStandardId);
        }

        public async Task<IEnumerable<QualityStandard>> GetQualityStandards()
        {
            return await _context.QualityStandards
                .Include(x => x.Properties)
                .Include(x => x.MaterialDefinitions)
                .ToListAsync();
        }

        public async Task<List<QualityStandard>> GetListByIdAsync(List<string> qualityStandardIds)
        {
            var qualityStandards = await _context.QualityStandards
                .Include(x => x.Properties)
                .Include(x => x.MaterialDefinitions)
                .Where(x => qualityStandardIds.Contains(x.ResourceId))
                .ToListAsync();

            var notFoundIds = qualityStandardIds
                .Where(id => !qualityStandards.Exists(pc => pc.ResourceId == id));

            if (notFoundIds.Any())
            {
                throw new EntitiesNotFoundException(nameof(MaterialDefinition), notFoundIds.ToList());
            }

            return qualityStandards;
        }
        public QualityStandard Update(QualityStandard qualityStandard)
        {
            return _context.QualityStandards
                    .Update(qualityStandard)
                    .Entity;
        }
    }
}
