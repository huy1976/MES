namespace MesMicroservice.Application.Queries.ManufacturingOrders;

public class ManufacturingOrderQueryHandler : IRequestHandler<ManufacturingOrderQuery, ManufacturingOrderDTO?>
{
    private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IProcessSegmentRepository _processSegmentRepository;
    private readonly IMapper _mapper;

    public ManufacturingOrderQueryHandler(IManufacturingOrderRepository manufacturingOrderRepository, IMapper mapper, IWorkOrderRepository workOrderRepository, IProcessSegmentRepository processSegmentRepository)
    {
        _manufacturingOrderRepository = manufacturingOrderRepository;
        _mapper = mapper;
        _workOrderRepository = workOrderRepository;
        _processSegmentRepository = processSegmentRepository;
    }

    public async Task<ManufacturingOrderDTO?> Handle(ManufacturingOrderQuery request, CancellationToken cancellationToken)
    {
        var manufacturingOrder = await _manufacturingOrderRepository.GetAsync(request.ManufacturingOrderId);
        if (manufacturingOrder is null)
        {
            throw new Exception($"Manufacturing Order with ID {request.ManufacturingOrderId} not found.");
        }

        var moDTO = new ManufacturingOrderDTO(manufacturingOrderId: manufacturingOrder.ManufacturingOrderId,
                                      materialDefinitionId: manufacturingOrder.MaterialDefinition.ResourceId,
                                      materialDefinitionName: manufacturingOrder.MaterialDefinition.Name,
                                      quantity: manufacturingOrder.Quantity,
                                      unit: manufacturingOrder.Unit,
                                      dueDate: manufacturingOrder.DueDate,
                                      customerId: manufacturingOrder.Customer.ResourceId,
                                      operationsDefinitionId: manufacturingOrder.OperationsDefinition.ResourceId,
                                      qualityStandardId: manufacturingOrder.QualityStandard.ResourceId,
                                      status: manufacturingOrder.Status.ToString(),
                                      createdBy: manufacturingOrder.CreatedBy.ResourceId,
                                      createDate: manufacturingOrder.CreateDate);

        var workOrderDTOs = new List<WorkOrderViewDTO>();
        foreach (var workOrder in manufacturingOrder.WorkOrders)
        {
            var segment = await _processSegmentRepository.GetAsync(workOrder.SegmentId);
            var workOrderDTO = new WorkOrderViewDTO(manufacturingOrderId: manufacturingOrder.Id,
                                                    manufacturingOrderCode: manufacturingOrder.ManufacturingOrderId,
                                                    id: workOrder.Id,
                                                    workOrderId: workOrder.WorkOrderId,
                                                    segmentId: workOrder.SegmentId,
                                                    sequenceNumber: workOrder.SequenceNumber,
                                                    materialDefinitionId: manufacturingOrder.MaterialDefinition.ResourceId,
                                                    productionTarget: workOrder.ProductionTarget,
                                                    unit: workOrder.Unit,
                                                    start: workOrder.StartTime,
                                                    end: workOrder.EndTime,
                                                    productionParamId: workOrder.OperationsSegment.ProductionParameterId,
                                                    qualityParamId: manufacturingOrder.QualityStandard.ResourceId,
                                                    status: workOrder.WorkOrderStatus.ToString());

            workOrderDTO.UpdateSegmentName(segment?.Description);
            var wo = await _workOrderRepository.GetByWoIdAsync(workOrder.WorkOrderId);
            if (workOrder is null)
            {
                throw new Exception($"Work Order with ID {workOrder.WorkOrderId} not found.");
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
                var materialDTO = new MaterialWorkOrderDTO(material.Id, material.MaterialDefinition.ResourceId, material.ClassName, material.Quantity, material.ValueUnitOfMeasure);
                materialDTOs.Add(materialDTO);
            }

            workOrderDTO.Equipments = equipmentDTOs;
            workOrderDTO.Persons = personDTOs;
            workOrderDTO.Materials = materialDTOs;


            workOrderDTOs.Add(workOrderDTO);
        }

        moDTO.workOrderDTOs = workOrderDTOs;

        return moDTO;
    }
}
