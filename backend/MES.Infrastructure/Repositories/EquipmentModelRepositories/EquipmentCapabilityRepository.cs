global using MesMicroservice.Domain.AggregateModels.EquipmentCapabilityAggregate;

namespace MesMicroservice.Infrastructure.Repositories.EquipmentModelRepositories
{
    public class EquipmentCapabilityRepository : BaseRepository, IEquipmentCapabilityRepository
    {
        public EquipmentCapabilityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<EquipmentCapability> Add(EquipmentCapability equipmentCapability)
        {
            if (equipmentCapability.IsTransient())
            {
                if (await ExistsAsync(equipmentCapability.ResourceId))
                {
                    throw new EntityDuplicationException($"EquipmentCapability with id {equipmentCapability.Id} already exists.");
                }

                return _context.EquipmentCapabilities
                    .Add(equipmentCapability)
                    .Entity;
            }
            else
            {
                return equipmentCapability;
            }
        }

        public async Task<EquipmentCapability?> GetAsync(string equipmentCapabilityId)
        {
            return await _context.EquipmentCapabilities
                .Include(x => x.Properties)
                .Include(x => x.Equipment)
                .FirstOrDefaultAsync(x => x.ResourceId == equipmentCapabilityId);
        }
        public async Task<IEnumerable<EquipmentCapability>> GetEquipmentCapabilities()
        {
            return await _context.EquipmentCapabilities
                 .Include(x => x.Properties)
                 .Include(x => x.Equipment)
                 .ToListAsync();
        }

        public EquipmentCapability Update(EquipmentCapability equipmentCapability)
        {
            return _context.EquipmentCapabilities
                    .Update(equipmentCapability)
                    .Entity;
        }

        public async Task<bool> ExistsAsync(string equipmentCapabilityId)
        {
            return await _context.EquipmentCapabilities
                .AnyAsync(x => x.ResourceId == equipmentCapabilityId);
        }

        public async Task DeleteAsync(string equipmentCapabilityId)
        {
            var equipmentCapability = await _context.EquipmentCapabilities
                .FirstOrDefaultAsync(x => x.ResourceId == equipmentCapabilityId);

            if (equipmentCapability is not null)
            {
                _context.EquipmentCapabilities.Remove(equipmentCapability);
            }
        }

    }
}
