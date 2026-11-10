
namespace MesMicroservice.Infrastructure.Repositories.OrderModelRepositories;
public class ManufacturingOrderRecordRepository : BaseRepository, IManufacturingRecordRepository
{
    public ManufacturingOrderRecordRepository(ApplicationDbContext context) : base(context)
    {
    }

    public ManufacturingRecord Add(ManufacturingRecord record)
    {
        if (record.IsTransient())
        {
            return _context.ManufacturingRecords
                .Add(record)
                .Entity;
        }
        else
        {
            return record;
        }
    }

    public async Task<IEnumerable<ManufacturingRecord>> GetAllAsync()
    {
        return await _context.ManufacturingRecords
                   .Include(x => x.Equipments)
                   .Include(x => x.WorkOrder)
                   .Include(x => x.OutputMaterialDefinition)
                   .ToListAsync();
    }
}
