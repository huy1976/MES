
namespace MesMicroservice.Infrastructure.Repositories.SegmentModelRepositories
{
    public class ProcessSegmentRepository : BaseRepository, IProcessSegmentRepository
    {
        public ProcessSegmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ProcessSegment> Add(ProcessSegment processSegment)
        {
            if (processSegment.IsTransient())
            {
                if (await ExistsAsync(processSegment.ResourceId))
                {
                    throw new EntityDuplicationException(nameof(MaterialDefinition), processSegment.ResourceId);
                }

                return _context.ProcessSegments
                    .Add(processSegment)
                    .Entity;
            }
            else
            {
                return processSegment;
            }
        }

        public async Task DeleteAsync(string processSegmentId)
        {
            var processSegment = await _context.ProcessSegments
                .FirstOrDefaultAsync(x => x.ResourceId == processSegmentId);

            if (processSegment is not null)
            {
                _context.ProcessSegments.Remove(processSegment);
            }
        }

        public async Task<bool> ExistsAsync(string processSegmentId)
        {
            return await _context.ProcessSegments
                .AnyAsync(x => x.ResourceId == processSegmentId);
        }

        public async Task<ProcessSegment?> GetAsync(string processSegmentId)
        {
            return await _context.ProcessSegments
                .Include(x => x.Duration)
                .Include(x => x.ProductionParameter)
                .Include(x => x.EquipmentSegmentSpecifications)
                    .ThenInclude(es => es.Properties)
                .Include(x => x.EquipmentSegmentSpecifications)
                    .ThenInclude(es => es.EquipmentClass)
                .Include(x => x.MaterialSegmentSpecifications)
                    .ThenInclude(x => x.Properties)
                .Include(x => x.MaterialSegmentSpecifications)
                    .ThenInclude(x => x.MaterialDefinition)
                .Include(x => x.PersonnelSegmentSpecifications)
                    .ThenInclude(es => es.Properties)
                .Include(x => x.PersonnelSegmentSpecifications)
                    .ThenInclude(es => es.PersonnelClass)
                .FirstOrDefaultAsync(x => x.ResourceId == processSegmentId);
        }


        public async Task<IEnumerable<ProcessSegment>> GetProcessSegments()
        {
            return await _context.ProcessSegments
               .Include(x => x.Duration)
               .Include(x => x.ProductionParameter)
               .Include(x => x.EquipmentSegmentSpecifications)
                   .ThenInclude(x => x.Properties)
               .Include(x => x.MaterialSegmentSpecifications)
                   .ThenInclude(x => x.Properties)
               .Include(x => x.MaterialSegmentSpecifications)
                   .ThenInclude(x => x.MaterialDefinition)
               .Include(x => x.PersonnelSegmentSpecifications)
                   .ThenInclude(x => x.Properties)
               .ToListAsync();
        }


        public async Task<List<ProcessSegment>> GetListByIdAsync(List<string> processSegmentIds)
        {
            var processSegments = await _context.ProcessSegments
                .Include(x => x.Duration)
                .Include(x => x.ProductionParameter)
                .Include(x => x.EquipmentSegmentSpecifications)
                    .ThenInclude(es => es.Properties)
                .Include(x => x.EquipmentSegmentSpecifications)
                    .ThenInclude(es => es.EquipmentClass)
                .Include(x => x.MaterialSegmentSpecifications)
                    .ThenInclude(es => es.Properties)
                .Include(x => x.MaterialSegmentSpecifications)
                    .ThenInclude(es => es.MaterialDefinition)
                .Include(x => x.PersonnelSegmentSpecifications)
                    .ThenInclude(es => es.Properties)
                .Include(x => x.PersonnelSegmentSpecifications)
                    .ThenInclude(es => es.PersonnelClass)
                .Where(x => processSegmentIds.Contains(x.ResourceId))
                .ToListAsync();

            var notFoundIds = processSegmentIds
                .Where(id => !processSegments.Exists(pc => pc.ResourceId == id));

            if (notFoundIds.Any())
            {
                throw new EntitiesNotFoundException(nameof(MaterialDefinition), notFoundIds.ToList());
            }

            return processSegments;
        }


        public ProcessSegment Update(ProcessSegment processSegment)
        {
            return _context.ProcessSegments
                    .Update(processSegment)
                    .Entity;
        }
    }
}
