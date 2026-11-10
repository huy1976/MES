
namespace MesMicroservice.Infrastructure.Repositories.ProductionParameterRepositories
{
    public class ProductionParameterRepository : BaseRepository, IProductionParameterRepository
    {
        public ProductionParameterRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ProductionParameter> Add(ProductionParameter productionParameter)
        {
            if (productionParameter.IsTransient())
            {
                if (await ExistsAsync(productionParameter.ResourceId))
                {
                    throw new EntityDuplicationException(nameof(MaterialDefinition), productionParameter.ResourceId);
                }

                return _context.ProductionParameters
                    .Add(productionParameter)
                    .Entity;
            }
            else
            {
                return productionParameter;
            }
        }

        public async Task DeleteAsync(string productionParameterId)
        {
            var productionParameter = await _context.ProductionParameters
                .FirstOrDefaultAsync(x => x.ResourceId == productionParameterId);

            if (productionParameter is not null)
            {
                _context.ProductionParameters.Remove(productionParameter);
            }
        }

        public async Task<bool> ExistsAsync(string productionParameterId)
        {
            return await _context.ProductionParameters
                .AnyAsync(x => x.ResourceId == productionParameterId);
        }

        public async Task<ProductionParameter?> GetAsync(string productionParameterId)
        {
            return await _context.ProductionParameters
                .Include(x => x.Properties)
                .FirstOrDefaultAsync(x => x.ResourceId == productionParameterId);
        }

        public async Task<IEnumerable<ProductionParameter>> GetProductionParameters()
        {
            return await _context.ProductionParameters
                .Include(x => x.Properties)
                .ToListAsync();
        }



        public async Task<List<ProductionParameter>> GetListByIdAsync(List<string> productionParameterIds)
        {
            var productionParameters = await _context.ProductionParameters
                .Include(x => x.Properties)
                .Where(x => productionParameterIds.Contains(x.ResourceId))
                .ToListAsync();

            var notFoundIds = productionParameterIds
                .Where(id => !productionParameters.Exists(pc => pc.ResourceId == id));

            if (notFoundIds.Any())
            {
                throw new EntitiesNotFoundException(nameof(MaterialDefinition), notFoundIds.ToList());
            }

            return productionParameters;
        }

        public ProductionParameter Update(ProductionParameter productionParameter)
        {
            return _context.ProductionParameters
                    .Update(productionParameter)
                    .Entity;
        }
    }
}
