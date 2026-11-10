namespace MesMicroservice.Application.Queries.WorkOrders
{
    public class WorkOrderByStatusesQueryHandler : IRequestHandler<WorkOrderByStatusesQuery, List<WorkOrderViewDTO>>
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
        private readonly IProcessSegmentRepository _processSegmentRepository;
        private readonly IMapper _mapper;

        public WorkOrderByStatusesQueryHandler(IWorkOrderRepository workOrderRepository, IManufacturingOrderRepository manufacturingOrderRepository, IProcessSegmentRepository processSegmentRepository, IMapper mapper)
        {
            _workOrderRepository = workOrderRepository;
            _manufacturingOrderRepository = manufacturingOrderRepository;
            _processSegmentRepository = processSegmentRepository;
            _mapper = mapper;
        }

        public async Task<List<WorkOrderViewDTO>> Handle(WorkOrderByStatusesQuery request, CancellationToken cancellationToken)
        {
            var workOrders = await _workOrderRepository.GetAll();
            var listOfWorkOrders = new List<WorkOrder>();
            foreach (var status in request.Statuses)
            {
                listOfWorkOrders.AddRange(workOrders.Where(wo => wo.WorkOrderStatus == status).ToList());
            }

            var workOrderDTOs = new List<WorkOrderViewDTO>();

            foreach (var wo in listOfWorkOrders)
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

                var processSegment = await _processSegmentRepository.GetAsync(wo.OperationsSegment.SegmentId);
                if (processSegment is null) throw new ResourceNotFoundException(nameof(ProcessSegment), wo.SegmentId);

                var manufacturingOrder = await _manufacturingOrderRepository.GetByIdAsync(wo.ManufacturingOrder.ManufacturingOrderId);

                // So sánh materialDefinitionId để lấy Items trong BOM VÀ CẬP NHẬT TÊN SEGMENT
                foreach (var item in manufacturingOrder.OperationsDefinition.OperationsMaterialBill.Items)
                {
                    foreach (var materialSpecification in wo.OperationsSegment.MaterialSpecifications)
                    {
                        if (item.MaterialDefinitionId == materialSpecification.MaterialDefinitionId)
                        {
                            woDTO.AddBOMItem(item.BillId);
                        }
                    }

                }

                woDTO.UpdateSegmentName(processSegment.Name);

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

                workOrderDTOs.Add(woDTO);
            }

            return workOrderDTOs;
        }
    }
}
