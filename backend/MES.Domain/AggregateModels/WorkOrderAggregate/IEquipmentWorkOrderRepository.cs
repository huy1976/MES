namespace MesMicroservice.Domain.AggregateModels.WorkOrderAggregate
{
    public interface IEquipmentWorkOrderRepository : IRepository<EquipmentWorkOrder>
    {
        public Task<IEnumerable<EquipmentWorkOrder>> GetEquipmentCurrentWorkOrdersAsync(int equipmentId);
    }
}
