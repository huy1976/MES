
namespace MesMicroservice.Infrastructure.Repositories.OrderModelRepositories
{
    public class WorkOrderErrorRecordRepository : BaseRepository, IWorkOrderErrorRecord
    {
        public WorkOrderErrorRecordRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<WorkOrderErrorRecord> Add(WorkOrderErrorRecord record)
        {
            return _context.WorkOrderErrorRecords
                .Add(record)
                .Entity;
        }

        public async Task<List<WorkOrderErrorRecord>> GetAllAsync()
        {
            return _context.WorkOrderErrorRecords .ToList();
        }

        public async Task<List<WorkOrderErrorRecord>> GetByWorkOrderId(string Id)
        {
            return await _context.WorkOrderErrorRecords
                .Where(r => r.WorkOrderId == Id)
                .OrderByDescending(r => r.Timestamp)
                .ToListAsync();
        }
    }

}
