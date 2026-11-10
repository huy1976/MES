using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MesMicroservice.Application.DTOs.WorkOrderDTOs.WorkOrderViewDTO;

namespace MesMicroservice.Application.Queries.Scheduling
{
    public class CurrentProductionScheduleQueryHandler
        : IRequestHandler<CurrentProductionScheduleQuery, List<WorkOrderViewDTO>>
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IProcessSegmentRepository _processSegmentRepository;

        public CurrentProductionScheduleQueryHandler(
            IWorkOrderRepository workOrderRepository,
            IProcessSegmentRepository processSegmentRepository)
        {
            _workOrderRepository = workOrderRepository;
            _processSegmentRepository = processSegmentRepository;
        }

        public async Task<List<WorkOrderViewDTO>> Handle(
            CurrentProductionScheduleQuery request,
            CancellationToken cancellationToken)
        {
            var allWorkOrders = await _workOrderRepository.GetAll();

            var scheduledWorkOrders = allWorkOrders
                .Where(wo =>
                    request.Statuses.Contains(wo.WorkOrderStatus)
                    && wo.StartTime.HasValue
                    && wo.EndTime.HasValue
                    && wo.StartTime.Value != DateTime.MinValue
                    && wo.EndTime.Value != DateTime.MinValue)
                .OrderBy(wo => wo.StartTime)
                .ToList();

            var result = new List<WorkOrderViewDTO>();

            foreach (var wo in scheduledWorkOrders)
            {
                var detailWo = await _workOrderRepository.GetByWoIdAsync(wo.WorkOrderId);
                if (detailWo == null)
                    continue;

                var dto = new WorkOrderViewDTO(
                    manufacturingOrderId: detailWo.ManufacturingOrder.Id,
                    manufacturingOrderCode: detailWo.ManufacturingOrder.ManufacturingOrderId,
                    id: detailWo.Id,
                    workOrderId: detailWo.WorkOrderId,
                    segmentId: detailWo.SegmentId,
                    sequenceNumber: detailWo.SequenceNumber,
                    materialDefinitionId: detailWo.MaterialDefinition.ResourceId,
                    productionTarget: detailWo.ProductionTarget,
                    unit: detailWo.Unit,
                    start: detailWo.StartTime,
                    end: detailWo.EndTime,
                    productionParamId: detailWo.OperationsSegment.ProductionParameterId,
                    qualityParamId: detailWo.ManufacturingOrder.QualityStandard.ResourceId,
                    status: detailWo.WorkOrderStatus.ToString()
                );

                dto.ActuallyStartTime = detailWo.ActuallyStartTime;
                dto.ActuallyEndTime = detailWo.ActuallyEndTime;
                dto.Duration = detailWo.Duration;
                dto.SetupTime = detailWo.SetupTime;
                dto.RunTime = detailWo.RunTime;
                var equipmentId = detailWo.EquipmentWorkOrders
    .FirstOrDefault()
    ?.Equipment
    ?.ResourceId ?? "Chưa gán máy";

                bool HasValidDate(DateTime? value)
                {
                    return value.HasValue && value.Value != DateTime.MinValue;
                }

                // Thanh kế hoạch: luôn vẽ nếu có Start/End kế hoạch
                if (HasValidDate(detailWo.StartTime) && HasValidDate(detailWo.EndTime))
                {
                    dto.GanttBars.Add(new GanttBarDTO
                    {
                        Type = "Plan",
                        Lane = $"{equipmentId} - Kế hoạch",
                        EquipmentId = equipmentId,
                        StartTime = detailWo.StartTime!.Value,
                        EndTime = detailWo.EndTime!.Value,
                        IsRunning = false,
                        IsCompleted = detailWo.WorkOrderStatus == EStatus.Completed,
                        IsDelayed = false,
                        Color = "#94a3b8"
                    });
                }

                // Thanh thực tế: chỉ vẽ khi lệnh đã thực sự bắt đầu
                if (HasValidDate(detailWo.ActuallyStartTime))
                {
                    var actualEnd = HasValidDate(detailWo.ActuallyEndTime)
                        ? detailWo.ActuallyEndTime!.Value
                        : DateTime.Now;

                    var plannedEnd = HasValidDate(detailWo.EndTime)
                        ? detailWo.EndTime!.Value
                        : actualEnd;

                    dto.GanttBars.Add(new GanttBarDTO
                    {
                        Type = "Actual",
                        Lane = $"{equipmentId} - Thực tế",
                        EquipmentId = equipmentId,
                        StartTime = detailWo.ActuallyStartTime!.Value,
                        EndTime = actualEnd,
                        IsRunning = detailWo.WorkOrderStatus == EStatus.InProduction && !HasValidDate(detailWo.ActuallyEndTime),
                        IsCompleted = detailWo.WorkOrderStatus == EStatus.Completed,
                        IsDelayed = actualEnd > plannedEnd,
                        Color = actualEnd > plannedEnd ? "#dc2626" : "#16a34a"
                    });
                }
                var processSegment = await _processSegmentRepository.GetAsync(detailWo.OperationsSegment.SegmentId);
                dto.UpdateSegmentName(processSegment?.Name);

                foreach (var equipment in detailWo.EquipmentWorkOrders)
                {
                    dto.Equipments.Add(new EquipmentWorkOrderDTO(
                        equipment.Id,
                        equipment.Equipment.ResourceId,
                        equipment.ClassName
                    ));
                }

                foreach (var person in detailWo.PersonWorkOrders)
                {
                    dto.Persons.Add(new PersonWorkOrderDTO(
                        person.Id,
                        person.Person.ResourceId,
                        person.ClassName,
                        person.ShiftName
                    ));
                }

                foreach (var material in detailWo.MaterialWorkOrders)
                {
                    dto.Materials.Add(new MaterialWorkOrderDTO(
                        material.Id,
                        material.MaterialDefinition.ResourceId,
                        material.ClassName,
                        material.Quantity,
                        material.ValueUnitOfMeasure
                    ));
                }

                var bomWo = await _workOrderRepository.GetListBomByWoIdAsync(detailWo.WorkOrderId);
                if (bomWo?.OperationsSegment?.OperationsDefinition?.OperationsMaterialBill?.Items != null)
                {
                    foreach (var item in bomWo.OperationsSegment.OperationsDefinition.OperationsMaterialBill.Items)
                    {
                        foreach (var materialSpecification in bomWo.OperationsSegment.MaterialSpecifications)
                        {
                            if (item.MaterialDefinitionId == materialSpecification.MaterialDefinitionId)
                            {
                                dto.AddBOMItem(item.BillId);
                            }
                        }
                    }
                }

                result.Add(dto);
            }
            return result;
        }
    }
}
