namespace MesMicroservice.Application.Commands.Equipments
{
    public class UpdateEquipmentStatusCommandHandler
        : IRequestHandler<UpdateEquipmentStatusCommand, bool>
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IEquipmentRepository _equipmentRepository;

        public UpdateEquipmentStatusCommandHandler(
            IWorkOrderRepository workOrderRepository,
            IEquipmentRepository equipmentRepository)
        {
            _workOrderRepository = workOrderRepository;
            _equipmentRepository = equipmentRepository;
        }

        public async Task<bool> Handle(
            UpdateEquipmentStatusCommand request,
            CancellationToken cancellationToken)
        {
            var workOrders = (await _workOrderRepository.GetAlAsync())
                .OrderBy(wo => wo.StartTime)
                .ToList();

            var equipments = (await _equipmentRepository.GetAllAsync())
                .Where(e => e.EquipmentStatus != EStatus.Maintenance
                         && e.EquipmentStatus != EStatus.Breakdown)
                .ToList();

            // Map WO Status -> Equipment Status
            var statusMap = new Dictionary<EStatus, EStatus>
            {
                { EStatus.InProduction, EStatus.Running },
                { EStatus.ReadyForProduction, EStatus.Planned },
                { EStatus.Completed, EStatus.Available }
            };

            // Lưu trạng thái "tốt nhất" cho mỗi Equipment
            var equipmentStatusResult = new Dictionary<string, EStatus>();

            foreach (var wo in workOrders)
            {
                if (!statusMap.TryGetValue(wo.WorkOrderStatus, out var mappedStatus))
                    continue;

                foreach (var ewo in wo.EquipmentWorkOrders)
                {
                    var equipmentId = ewo.Equipment.ResourceId;

                    if (!equipmentStatusResult.ContainsKey(equipmentId) ||
                        GetPriority(mappedStatus) >
                        GetPriority(equipmentStatusResult[equipmentId]))
                    {
                        equipmentStatusResult[equipmentId] = mappedStatus;
                    }
                }
            }

            // Apply status cho Equipment
            foreach (var equipment in equipments)
            {
                if (equipment.EquipmentStatus == EStatus.Running)
                    continue; // không override máy đang chạy

                if (equipmentStatusResult.TryGetValue(
                        equipment.ResourceId,
                        out var newStatus))
                {
                    equipment.UpdateStatus(newStatus);
                }
                // ❗ KHÔNG reset Available nếu không có WO
            }

            return await _equipmentRepository
                .UnitOfWork
                .SaveEntitiesAsync(cancellationToken);
        }

        private static int GetPriority(EStatus status) => status switch
        {
            EStatus.Running => 3,
            EStatus.Planned => 2,
            EStatus.Available => 1,
            _ => 0
        };
    }
}
