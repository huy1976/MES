namespace MesMicroservice.Application.Queries.WorkOrders;

public class WorkOrdersQueryHandler : IRequestHandler<WorkOrdersQuery, List<WorkOrderViewDTO>>
{
    private readonly IWorkOrderRepository _workOrderRepository;

    public WorkOrdersQueryHandler(IWorkOrderRepository workOrderRepository)
    {
        _workOrderRepository = workOrderRepository;
    }

    public async Task<List<WorkOrderViewDTO>> Handle(WorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var wos = await _workOrderRepository.GetAll();

        if (request.IdStartedWith is not null)
        {
            wos = wos.Where(x => x.WorkOrderId.StartsWith(request.IdStartedWith));
        }

        if (request.ManufacturingOrderId is not null)
        {
            wos = wos.Where(x => x.ManufacturingOrder.ManufacturingOrderId == request.ManufacturingOrderId);
        }

        if (request.Status is not null)
        {
            wos = wos.Where(x => x.WorkOrderStatus == request.Status);
        }

        if (request.WorkCenterId is not null)
        {
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            wos = wos.Where(x => x.WorkCenter != null && x.WorkCenter.HierarchyModelId == request.WorkCenterId);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8604 // Possible null reference argument.
        }

        wos = wos
            .OrderBy(x => x.WorkOrderId)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize);

        var woDTOs = new List<WorkOrderViewDTO>();
        foreach (var wo in wos)
        {
            var woDTO = new WorkOrderViewDTO(manufacturingOrderId: wo.ManufacturingOrder.Id,
                                             manufacturingOrderCode: wo.ManufacturingOrder.ManufacturingOrderId,
                                             id: wo.Id,
                                             workOrderId: wo.WorkOrderId,
                                             segmentId: wo.SegmentId,
                                             sequenceNumber: wo.SequenceNumber,
                                             materialDefinitionId: wo.MaterialDefinition.ResourceId,
                                             productionTarget: wo.ProductionTarget,
                                             unit: wo.Unit,
                                             start: wo.StartTime,
                                             end: wo.EndTime,
                                             productionParamId: wo.OperationsSegment.ProductionParameterId,
                                             qualityParamId: wo.ManufacturingOrder.QualityStandard.ResourceId,
                                             status: wo.WorkOrderStatus.ToString());

            var workOrder = await _workOrderRepository.GetByWoIdAsync(wo.WorkOrderId);
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

            woDTO.Equipments = equipmentDTOs;
            woDTO.Persons = personDTOs;
            woDTO.Materials = materialDTOs;

            woDTOs.Add(woDTO);
        }

        return woDTOs;
    }
}
