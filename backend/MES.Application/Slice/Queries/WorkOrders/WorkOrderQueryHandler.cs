using MesMicroservice.Domain.AggregateModels.ManufacturingOrderAggregate;

namespace MesMicroservice.Application.Queries.WorkOrders;

public class WorkOrderQueryHandler : IRequestHandler<WorkOrderQuery, WorkOrderViewDTO?>
{
    private readonly IWorkOrderRepository _workOrderRepository;

    public WorkOrderQueryHandler(IWorkOrderRepository workOrderRepository)
    {
        _workOrderRepository = workOrderRepository;
    }

    public async Task<WorkOrderViewDTO?> Handle(WorkOrderQuery request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByWoIdAsync(request.WorkOrderId);
        if (workOrder is null)
        {
            throw new Exception($"Work Order with ID {request.WorkOrderId} not found.");
        }


        var workOrderDTO = new WorkOrderViewDTO(manufacturingOrderId: workOrder.ManufacturingOrder.Id,
                                                manufacturingOrderCode: workOrder.ManufacturingOrder.ManufacturingOrderId,
                                                id: workOrder.Id,
                                                workOrderId: workOrder.WorkOrderId,
                                                segmentId: workOrder.SegmentId,
                                                sequenceNumber: workOrder.SequenceNumber,
                                                materialDefinitionId: workOrder.MaterialDefinition.ResourceId,
                                                productionTarget: workOrder.ProductionTarget,
                                                unit: workOrder.Unit,
                                                start: workOrder.StartTime,
                                                end: workOrder.EndTime,
                                                productionParamId: workOrder.OperationsSegment.ProductionParameterId,
                                                qualityParamId: workOrder.ManufacturingOrder.QualityStandard.ResourceId,
                                                status: workOrder.WorkOrderStatus.ToString());

        
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
            var materialDTO = new MaterialWorkOrderDTO(material.Id, material.MaterialDefinition.ResourceId, material.ClassName, material.Quantity, material.ValueUnitOfMeasure);
            materialDTOs.Add(materialDTO);
        }

        workOrderDTO.Equipments = equipmentDTOs;
        workOrderDTO.Persons = personDTOs;
        workOrderDTO.Materials = materialDTOs;

        return workOrderDTO;
    }
}
