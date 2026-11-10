namespace MesMicroservice.Infrastructure.Repositories.OrderModelRepositories
{
    public class WorkOrderProgressRecordRepository : BaseRepository, IWorkOrderProgressRecord
    {
        public WorkOrderProgressRecordRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<WorkOrderProgressRecord> Add(WorkOrderProgressRecord record)
        {
            return _context.WorkOrderProgressRecords
                .Add(record)
                .Entity;
        }

        public async Task<List<WorkOrderProgressRecord>> GetByWorkOrderId(string Id)
        {
            return await _context.WorkOrderProgressRecords
                .Where(r => r.WorkOrderId == Id)
                .OrderByDescending(r => r.Timestamp)
                .ToListAsync();
        }
    }
}
