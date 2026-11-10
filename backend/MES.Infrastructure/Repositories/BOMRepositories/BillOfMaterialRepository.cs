using Microsoft.EntityFrameworkCore;

namespace MesMicroservice.Infrastructure.Repositories.BOMRepositories
{
    public class BillOfMaterialRepository : BaseRepository, IBillOfMaterialRepository
    {
        public BillOfMaterialRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<BillOfMaterial> Add(BillOfMaterial billOfMaterial)
        {
            if (billOfMaterial.IsTransient())
            {
                if (await ExistsAsync(billOfMaterial.ResourceId))
                {
                    throw new EntityDuplicationException(nameof(MaterialDefinition), billOfMaterial.ResourceId);
                }

                return _context.BillOfMaterials
                    .Add(billOfMaterial)
                    .Entity;
            }
            else
            {
                return billOfMaterial;
            }
        }

        public async Task DeleteAsync(string billOfMaterialId)
        {
            var billOfMaterial = await _context.BillOfMaterials
                .FirstOrDefaultAsync(x => x.ResourceId == billOfMaterialId);

            if (billOfMaterial is not null)
            {
                _context.BillOfMaterials.Remove(billOfMaterial);
            }
        }

        public async Task<bool> ExistsAsync(string billOfMaterialId)
        {
            return await _context.BillOfMaterials
                .AnyAsync(x => x.ResourceId == billOfMaterialId);
        }

        public async Task<BillOfMaterial?> GetAsync(string billOfMaterialId)
        {
            return await _context.BillOfMaterials
                .Include(x => x.MaterialDefinition)
                .FirstOrDefaultAsync(x => x.ResourceId == billOfMaterialId);
        }

        public async Task<IEnumerable<BillOfMaterial>> GetBillOfMaterials()
        {
            return await _context.BillOfMaterials
                .Include(x => x.MaterialDefinition)
                .ToListAsync();
        }

        public async Task<List<BillOfMaterial>> GetListByIdAsync(List<string> billOfMaterialIds)
        {
            var billOfMaterials = await _context.BillOfMaterials
                .Include(x => x.MaterialDefinition)
                .Where(x => billOfMaterialIds.Contains(x.ResourceId))
                .ToListAsync();

            var notFoundIds = billOfMaterialIds
                .Where(id => !billOfMaterials.Exists(pc => pc.ResourceId == id));

            if (notFoundIds.Any())
            {
                throw new EntitiesNotFoundException(nameof(MaterialDefinition), notFoundIds.ToList());
            }

            return billOfMaterials;
        }

        public BillOfMaterial Update(BillOfMaterial billOfMaterial)
        {
            return _context.BillOfMaterials
                    .Update(billOfMaterial)
                    .Entity;
        }



    }
}
