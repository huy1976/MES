using MesMicroservice.Domain.AggregateModels.DowntimeAggregate;

namespace MesMicroservice.Application.Commands.WorkOrders
{
    public class CreateWorkOrderErrorRecordByMinorStopCommandHandler : IRequestHandler<CreateWorkOrderErrorRecordByMinorStopCommand, bool>
    {
        private readonly IWorkOrderErrorRecord _workOrderErrorRecord;
        private readonly IWorkOrderProgressRecord _workOrderProgressRecord;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IDowntimeRepository _downtimeRepository;
        private List<Equipment> equipmentList = new List<Equipment>();
        public CreateWorkOrderErrorRecordByMinorStopCommandHandler(IWorkOrderErrorRecord workOrderErrorRecord, IWorkOrderProgressRecord workOrderProgressRecord, IEquipmentRepository equipmentRepository, IPersonRepository personRepository, IWorkOrderRepository workOrderRepository, IDowntimeRepository downtimeRepository)
        {
            _workOrderErrorRecord = workOrderErrorRecord;
            _workOrderProgressRecord = workOrderProgressRecord;
            _equipmentRepository = equipmentRepository;
            _personRepository = personRepository;
            _workOrderRepository = workOrderRepository;
            _downtimeRepository = downtimeRepository;
        }

        public async Task<bool> Handle(CreateWorkOrderErrorRecordByMinorStopCommand request, CancellationToken cancellationToken)
        {
            var handleBy = string.Empty;

            if (string.IsNullOrEmpty(request.ErrorId))
            {
                throw new ArgumentNullException(nameof(request.ErrorId), "ErrorId cannot be null or empty.");
            }

            if (request.HandledBy != string.Empty)
            {
                var person = await _personRepository.GetAsync(request.HandledBy);
                if (person is null)
                {
                    throw new ArgumentException($"HandledBy person with ID {request.HandledBy} does not exist.");
                }
                handleBy = person.ResourceId;
            }

            var reportedPerson = await _personRepository.GetAsync(request.ReportedBy);
            if (reportedPerson is null)
            {
                throw new ArgumentException($"ReportedBy person with ID {request.ReportedBy} does not exist.");
            }

            var errorRecord = new WorkOrderErrorRecord(errorId: request.ErrorId,
                                                       errorName: request.ErrorName,
                                                       status: (request.End == DateTime.MinValue) ? EStatus.Pending : EStatus.Completed,
                                                       downtimeType: request.DowntimeType,
                                                       notes: request.Notes ?? "--",
                                                       handledBy: handleBy ?? "--",
                                                       reportedBy: reportedPerson.ResourceId,
                                                       timestamp: DateTime.Now,
                                                       durationInHours: (request.End != DateTime.MinValue) ? (request.End - request.Start).TotalHours : 0,
                                                       start: request.Start != DateTime.MinValue ? request.Start : DateTime.Now,
                                                       end: request.End,
                                                       workOrderId: request.WorkOrderId,
                                                       isUpdateTime: request.IsUpdateTime ?? false,
                                                       isSchedule: request.IsSchedule ?? false);

            await _workOrderErrorRecord.Add(errorRecord);

            var wo = await _workOrderRepository.GetByWoIdAsync(request.WorkOrderId);
            if (wo is null)
            {
                throw new ArgumentException($"WorkOrder with ID {request.WorkOrderId} does not exist.");
            }

            foreach (var equipmentWorkOrder in wo.EquipmentWorkOrders)
            {
                var equipment = await _equipmentRepository.GetAsync(equipmentWorkOrder.Equipment.ResourceId);
                if (equipment is null) continue;
                equipmentList.Add(equipment);
            }

            var equipmentRunning = equipmentList.FirstOrDefault(x => x.EquipmentStatus == EStatus.Running);

            if (equipmentRunning is null)
            {
                throw new ArgumentException($"Equipment with Status {EStatus.Running} does not exist.");
            }

            var equipmentDowntimeType = new EquipmentDowntimeType(equipmentClassId: equipmentRunning.EquipmentClass.Id,
                                                                  name: equipmentRunning.Name,
                                                                  description : request.Notes ?? "--",
                                                                  type: request.DowntimeType
                                                                    );
            var downTime = new Downtime(equipment: equipmentRunning,
                                        equipmentId: equipmentRunning.Id,
                                        from: request.Start != DateTime.MinValue ? request.Start : DateTime.Now,
                                        to: request.End,
                                        downtimeType: equipmentDowntimeType
                                        );

            _downtimeRepository.Add(downTime);

            return await _workOrderErrorRecord.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }

    }
}
