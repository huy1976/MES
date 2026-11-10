namespace MesMicroservice.Application.Queries.WorkOrders
{
    public class WorkOrderProgressViewQueryHandler : IRequestHandler<WorkOrderProgressViewQuery, List<WorkOrderProgressViewDTO>>
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IWorkOrderProgressRecord _workOrderProgressRecordRepository;

        public WorkOrderProgressViewQueryHandler(IWorkOrderRepository workOrderRepository, IWorkOrderProgressRecord workOrderProgressRecordRepository)
        {
            _workOrderRepository = workOrderRepository;
            _workOrderProgressRecordRepository = workOrderProgressRecordRepository;
        }

        public async Task<List<WorkOrderProgressViewDTO>> Handle(WorkOrderProgressViewQuery request, CancellationToken cancellationToken)
        {
            var workOrders = await _workOrderRepository.GetAll();
            workOrders = workOrders
                .Where(wo => wo.WorkOrderStatus != EStatus.Pending && wo.WorkOrderStatus != EStatus.PendingScheduling)
                .OrderByDescending(wo => wo.StartTime)
                .ToList();

            var workOrderProgressViews = new List<WorkOrderProgressViewDTO>();

            foreach (var wo in workOrders)
            {
                var progressRecords = await _workOrderProgressRecordRepository.GetByWorkOrderId(wo.WorkOrderId);

                var nearPr = progressRecords
                    .OrderByDescending(r => r.Timestamp)
                    .FirstOrDefault();

                var progressByDay = progressRecords
                    .Where(r => r.Timestamp.Date == DateTime.Today.Date)
                    .ToList();

                var productionTargetByDay = 0.0;
                productionTargetByDay = CalculateDailyMaterial(
                    wo.StartTime ?? DateTime.MinValue,
                    wo.EndTime ?? DateTime.MinValue,
                    (double)wo.ProductionTarget,
                    DateTime.Today
                );

                var workOrderProgressView = new WorkOrderProgressViewDTO
                {
                    ManufacturingOrderId = wo.ManufacturingOrder.Id,
                    ManufacturingOrderCode = wo.ManufacturingOrder.ManufacturingOrderId,
                    Id = wo.Id,
                    WorkOrderId = wo.WorkOrderId,
                    Status = wo.WorkOrderStatus.ToString(),
                    SegmentId = wo.SegmentId,
                    SequenceNumber = wo.SequenceNumber,
                    MaterialDefinitionId = wo.MaterialDefinition.ResourceId,
                    ProductionTarget = wo.ProductionTarget,
                    ProductionTargetByDay = Math.Round(productionTargetByDay,2),
                    ProducedQuantity = nearPr is not null ? nearPr.ProducedQuantity : 0,
                    DeltaQuantity = nearPr is not null ? nearPr.DeltaQuantity : 0,
                    ProgressPercentage = nearPr is not null ? nearPr.ProgressPercentage : 0,
                    ErrorQuantity = nearPr is not null ? nearPr.ErrorQuantity : 0,
                    UnitOfMeasure = wo.Unit,
                    PlanedStart = wo.StartTime,
                    PlanedEnd = wo.EndTime,
                    ActualStartDate = wo.ActuallyStartTime != DateTime.MinValue ? wo.ActuallyStartTime : null,
                    ActualEndDate = wo.ActuallyEndTime != DateTime.MinValue ? wo.ActuallyEndTime : null,
                    ProductionParamId = wo.OperationsSegment.ProductionParameterId,
                    QualityParamId = wo.ManufacturingOrder.QualityStandard.ResourceId
                };

                var workOrder = await _workOrderRepository.GetByWoIdAsync(wo.WorkOrderId);
                if (workOrder is null)
                {
                    throw new Exception($"Work Order with ID {wo.WorkOrderId} not found.");
                }

                var equipmentDTOs = new List<EquipmentWorkOrderDTO>();
                var personDTOs = new List<PersonWorkOrderDTO>();
                var materialDTOs = new List<MaterialWorkOrderDTO>();
                foreach (var equipment in workOrder.EquipmentWorkOrders)
                {
                    var equipmentDTO = new EquipmentWorkOrderDTO(equipment.Id, equipment.Equipment.ResourceId, equipment.ClassName);
                    equipmentDTOs.Add(equipmentDTO);
                }
                foreach (var person in workOrder.PersonWorkOrders)
                {
                    var personDTO = new PersonWorkOrderDTO(person.Id, person.Person.ResourceId, person.ClassName, person.ShiftName);
                    personDTOs.Add(personDTO);
                }
                foreach (var material in workOrder.MaterialWorkOrders)
                {
                    var materialDTO = new MaterialWorkOrderDTO(material.Id, material.MaterialDefinition.ResourceId, 
                                                               material.ClassName, 
                                                               material.Quantity, 
                                                               material.ValueUnitOfMeasure);
                    if (wo.StartTime.HasValue && wo.EndTime.HasValue)
                    {
                        materialDTO.RequireRawComponentQuantityPerDay = Math.Round(CalculateDailyMaterial(
                            wo.StartTime.Value,
                            wo.EndTime.Value,
                            (double)material.Quantity.GetValue(),
                            DateTime.Today
                        ), 1);
                    }

                    var qty = progressByDay
                        .Where(pr => pr.RawConsumedQuatity > 0)
                        .Sum(pr => pr.DeltaRawConsumedQuatity);

                    materialDTO.ActualRawComponentQuantityPerDay = Math.Round(qty,2);

                    materialDTO.ActualRawComponentQuantity = Math.Round((nearPr is not null ? nearPr.RawConsumedQuatity : 0),1);

                    materialDTOs.Add(materialDTO);
                }

                workOrderProgressView.Equipments = equipmentDTOs;
                workOrderProgressView.Persons = personDTOs;
                workOrderProgressView.Materials = materialDTOs;

                workOrderProgressViews.Add(workOrderProgressView);
            };

            return workOrderProgressViews;
        }

        private static double CalculateDailyMaterial(DateTime woStart, DateTime woEnd, double totalMaterial, DateTime day)
        {
            var dayStart = day.Date;
            var dayEnd = day.Date.AddDays(1).AddTicks(-1);

            var overlapStart = woStart > dayStart ? woStart : dayStart;
            var overlapEnd = woEnd < dayEnd ? woEnd : dayEnd;

            if (overlapEnd <= overlapStart) return 0;

            double woTotalSeconds = (woEnd - woStart).TotalSeconds;
            if (woTotalSeconds <= 0) return 0;

            double overlapSeconds = (overlapEnd - overlapStart).TotalSeconds;
            return totalMaterial * (overlapSeconds / woTotalSeconds);
        }

    }
}
