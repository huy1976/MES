namespace MesMicroservice.Application.Commands.WorkOrders
{
    public class CreateWorkOrderProgressRecordCommandHandler : IRequestHandler<CreateWorkOrderProgressRecordCommand, bool>
    {
        private readonly IWorkOrderProgressRecord _workOrderProgressRecordRepository;
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
        private readonly IEquipmentRepository _equipmentRepository;

        public CreateWorkOrderProgressRecordCommandHandler(IWorkOrderProgressRecord workOrderProgressRecordRepository, IWorkOrderRepository workOrderRepository, IPersonRepository personRepository, IManufacturingOrderRepository manufacturingOrderRepository, IEquipmentRepository equipmentRepository)
        {
            _workOrderProgressRecordRepository = workOrderProgressRecordRepository;
            _workOrderRepository = workOrderRepository;
            _personRepository = personRepository;
            _manufacturingOrderRepository = manufacturingOrderRepository;
            _equipmentRepository = equipmentRepository;
        }

        public async Task<bool> Handle(CreateWorkOrderProgressRecordCommand request, CancellationToken cancellationToken)
        {
            var wo = await _workOrderRepository.GetByWoIdAsync(request.WorkOrderId);
            if (wo is null)
            {
                throw new Exception($"Work Order with ID {request.WorkOrderId} not found.");
            }

            var progressRecords = await _workOrderProgressRecordRepository.GetByWorkOrderId(wo.WorkOrderId);

            var nearPr = progressRecords
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefault();
            var person = await _personRepository.GetAsync(request.RecordedBy);
            if (person is null)
            {
                throw new Exception($"Person with ID {request.RecordedBy} not found.");
            }

            var deltaQuantity = request.DeltaQuantityProduced != 0 ? request.DeltaQuantityProduced : 0;
            var delateErrorQuantity = request.DeltaQuantityError != 0 ? request.DeltaQuantityError : 0;
            var deltaRawConsumedQuantity = request.DeltaRawConsumedQuatity != 0 ? request.DeltaRawConsumedQuatity : 0;
            var producedQuantity = (nearPr is not null ? nearPr.ProducedQuantity : 0) + deltaQuantity;
            var errorQuantity = (nearPr is not null ? nearPr.ErrorQuantity : 0) + delateErrorQuantity;
            var rawConsumedQuantity = (nearPr is not null ? nearPr.RawConsumedQuatity : 0) + deltaRawConsumedQuantity;
            var progressPercentage = wo.ProductionTarget != 0 ? ( (producedQuantity + errorQuantity) / ((double)wo.ProductionTarget)) * 100 : 0;
            var checkEnd = (producedQuantity + errorQuantity) >= ((double)wo.ProductionTarget);
            if (checkEnd)
            {
                progressPercentage = 100;
                wo.UpdateStatus(EStatus.Completed);
                var equipmentWorkOrders = wo.EquipmentWorkOrders;
                var personWorkOrders = wo.PersonWorkOrders;
                foreach (var ewo in equipmentWorkOrders)
                {
                    var equipment = await _equipmentRepository.GetAsync(ewo.Equipment.ResourceId);
                    if (equipment is null) continue;
                    equipment.UpdateStatus(EStatus.Available);
                }
                foreach (var personWorkOrder in personWorkOrders)
                {
                    var personWo = await _personRepository.GetAsync(personWorkOrder.Person.ResourceId);
                    if (personWo is null) continue;
                    personWo.UpdateStatus(EStatus.Available);
                }
                wo.UpdateActuallyTime(wo.ActuallyStartTime, request.Timestamp);
                await _manufacturingOrderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                await _manufacturingOrderRepository.UpdateStatusById(wo.ManufacturingOrder.ManufacturingOrderId);

            }

            var newRecord = new WorkOrderProgressRecord(progressPercentage: progressPercentage,
                                                        status: wo.WorkOrderStatus,
                                                        totalQuantity: ((double)wo.ProductionTarget),
                                                        producedQuantity: producedQuantity,
                                                        errorQuantity: errorQuantity,
                                                        deltaQuantity: deltaQuantity,
                                                        deltaErrorQuantity: delateErrorQuantity,
                                                        rawConsumedQuatity: rawConsumedQuantity,
                                                        deltaRawConsumedQuatity: deltaRawConsumedQuantity,
                                                        unitOfMeasure: wo.Unit,
                                                        actualStartDate: wo.ActuallyStartTime,
                                                        actualEndDate: wo.ActuallyEndTime,
                                                        timestamp: request.Timestamp ?? DateTime.Now,
                                                        note: string.Empty,
                                                        equipment: wo.EquipmentWorkOrders.Select(x => x.Equipment.ResourceId).FirstOrDefault() ?? null!,
                                                        workOrderId: wo.WorkOrderId,
                                                        updatedBy: person.ResourceId);

            await _workOrderProgressRecordRepository.Add(newRecord);

            return await _workOrderProgressRecordRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
