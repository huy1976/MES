namespace MesMicroservice.Infrastructure.Repositories.OrderModelRepositories
{
    public class EquipmentWorkOrderRepository : BaseRepository, IEquipmentWorkOrderRepository
    {
        public EquipmentWorkOrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EquipmentWorkOrder>> GetEquipmentCurrentWorkOrdersAsync(int equipmentId)
        {
            return await _context.EquipmentWorkOrders
                .Where(ewo =>
                    ewo.Equipment.Id == equipmentId &&
                    ewo.WorkOrder.WorkOrderStatus != EStatus.Completed)
                .Include(ewo => ewo.WorkOrder)
                .ToListAsync();
        }
    }
}
