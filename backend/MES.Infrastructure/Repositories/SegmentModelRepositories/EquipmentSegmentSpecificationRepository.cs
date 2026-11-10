
using MesMicroservice.Domain.AggregateModels.SegmentAggregate;

namespace MesMicroservice.Infrastructure.Repositories.SegmentModelRepositories
{
    public class EquipmentSegmentSpecificationRepository : BaseRepository, IEquipmentSegmentSpecificationRepository
    {
        public EquipmentSegmentSpecificationRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<EquipmentSegmentSpecification> Add(EquipmentSegmentSpecification equipmentSegmentSpecification)
        {
            if (equipmentSegmentSpecification.IsTransient())
            {
                if (await ExistsAsync(equipmentSegmentSpecification.ResourceId))
                {
                    throw new EntityDuplicationException(nameof(Equipment), equipmentSegmentSpecification.ResourceId);
                }

                return _context.EquipmentSegmentSpecifications
                    .Add(equipmentSegmentSpecification)
                    .Entity;
            }
            else
            {
                return equipmentSegmentSpecification;
            }
        }

        public async Task DeleteAsync(string equipmentSegmentSpecificationId)
        {
            var equipmentSegmentSpecification = await _context.EquipmentSegmentSpecifications
                .FirstOrDefaultAsync(x => x.ResourceId == equipmentSegmentSpecificationId);

            if (equipmentSegmentSpecification is not null)
            {
                _context.EquipmentSegmentSpecifications.Remove(equipmentSegmentSpecification);
            }
        }

        public async Task<bool> ExistsAsync(string equipmentSegmentSpecificationId)
        {
            return await _context.EquipmentSegmentSpecifications
                .AnyAsync(x => x.ResourceId == equipmentSegmentSpecificationId);
        }

        public async Task<EquipmentSegmentSpecification?> GetAsync(string equipmentSegmentSpecificationId)
        {
            return await _context.EquipmentSegmentSpecifications
                .Include(x => x.Properties)
                .Include(x => x.ProcessSegment)
                .Include(x => x.EquipmentClass)
                .FirstOrDefaultAsync(x => x.ResourceId == equipmentSegmentSpecificationId);
        }

        public async Task<IEnumerable<EquipmentSegmentSpecification>> GetEquipmentSegmentSpecifications()
        {
            return await _context.EquipmentSegmentSpecifications
                .Include(x => x.Properties)
                .Include(x => x.ProcessSegment)
                .Include(x => x.EquipmentClass)
                .ToListAsync();
        }

        public async Task<IEnumerable<EquipmentSegmentSpecification>> GetEquipmentSegmentSpecificationsByProcessSegmentIdAsync(string processSegmentId)
        {
            return await _context.EquipmentSegmentSpecifications
            .Include(x => x.Properties)
            .Include(x => x.ProcessSegment)
            .Include(x => x.EquipmentClass)
            .Where(x => x.ProcessSegment.ResourceId == processSegmentId)
            .ToListAsync();
        }

        public async Task<List<EquipmentSegmentSpecification>> GetListByIdAsync(List<string> equipmentSegmentSpecificationIds)
        {
            var equipmentSegmentSpecifications = await _context.EquipmentSegmentSpecifications
                .Include(x => x.Properties)
                .Include(x => x.ProcessSegment)
                .Include(x => x.EquipmentClass)
                .Where(x => equipmentSegmentSpecificationIds.Contains(x.ResourceId))
                .ToListAsync();

            var notFoundIds = equipmentSegmentSpecificationIds
                .Where(id => !equipmentSegmentSpecifications.Exists(pc => pc.ResourceId == id));

            if (notFoundIds.Any())
            {
                throw new EntitiesNotFoundException(nameof(Equipment), notFoundIds.ToList());
            }

            return equipmentSegmentSpecifications;
        }

        public EquipmentSegmentSpecification Update(EquipmentSegmentSpecification equipmentSegmentSpecification)
        {
            return _context.EquipmentSegmentSpecifications
                    .Update(equipmentSegmentSpecification)
                    .Entity;
        }


    }
}
