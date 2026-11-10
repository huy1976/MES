namespace MesMicroservice.Application.Queries.WorkOrders
{
    public class ReviewNewScheduleTimeForWoQueryHandler : IRequestHandler<ReviewNewScheduleTimeForWoQuery, List<ReScheduleTimeForWODTO>>
    {
        private readonly IErrorService _errorService;
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IEquipmentRepository _equipmentRepository;

        public ReviewNewScheduleTimeForWoQueryHandler(IErrorService errorService, IWorkOrderRepository workOrderRepository, IEquipmentRepository equipmentRepository)
        {
            _errorService = errorService;
            _workOrderRepository = workOrderRepository;
            _equipmentRepository = equipmentRepository;
        }

        public async Task<List<ReScheduleTimeForWODTO>> Handle(ReviewNewScheduleTimeForWoQuery request, CancellationToken cancellationToken)
        {
            var rescheduleList = new List<ReScheduleTimeForWODTO>();

            var wo = await _workOrderRepository.GetByWoIdAsync(request.WoId);
            if (wo is null)
            {
                throw new ArgumentException($"WorkOrder with ID {request.WoId} does not exist.");
            }
            foreach (var equipmentWorkOrder in wo.EquipmentWorkOrders)
            {
                var equipment = await _equipmentRepository.GetAsync(equipmentWorkOrder.Equipment.ResourceId);
                if (equipment is null) continue;
                equipment.UpdateStatus(EStatus.Breakdown);
            }

            wo.TeardownTime += request.Duration;
            if (wo.EndTime.HasValue)
            {
                var endtime = wo.EndTime.Value.AddHours(request.Duration);
                wo.UpdatePlanTime(wo.StartTime, endtime);
            }

            foreach (var equipmentWorkOrder in wo.EquipmentWorkOrders)
            {
                var equipment = await _equipmentRepository.GetAsync(equipmentWorkOrder.Equipment.ResourceId);
                if (equipment is null) continue;
                equipment.UpdateStatus(EStatus.Running);
            }
            var wos = await _workOrderRepository.GetListByStatus(EStatus.ReadyForProduction);
            rescheduleList = _errorService.UpdateTimeStamp(wo, wos, request.Duration);

            return rescheduleList;
        }

    }
}
