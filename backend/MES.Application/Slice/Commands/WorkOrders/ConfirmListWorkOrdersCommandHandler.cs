using MesMicroservice.Domain.AggregateModels.SegmentAggregate;

namespace MesMicroservice.Application.Commands.WorkOrders
{
    public class ConfirmListWorkOrdersCommandHandler : IRequestHandler<ConfirmListWorkOrdersCommand, bool>
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
        private readonly IProcessSegmentRepository _processSegmentRepository;
        private readonly IMapper _mapper;

        public ConfirmListWorkOrdersCommandHandler(IWorkOrderRepository workOrderRepository, IEquipmentRepository equipmentRepository, IPersonRepository personRepository, IMaterialDefinitionRepository materialDefinitionRepository, IManufacturingOrderRepository manufacturingOrderRepository, IMapper mapper, IProcessSegmentRepository processSegmentRepository)
        {
            _workOrderRepository = workOrderRepository;
            _equipmentRepository = equipmentRepository;
            _personRepository = personRepository;
            _materialDefinitionRepository = materialDefinitionRepository;
            _manufacturingOrderRepository = manufacturingOrderRepository;
            _mapper = mapper;
            _processSegmentRepository = processSegmentRepository;
        }

        public async Task<bool> Handle(ConfirmListWorkOrdersCommand request, CancellationToken cancellationToken)
        {
            var workOrderIds = request.ScheduledWorkOrderDTOs.Select(wo => wo.WorkOrderId).ToList();

            var workOrders = await _workOrderRepository.GetByWoIds(workOrderIds);

            foreach (var scheduledWoDto in request.ScheduledWorkOrderDTOs)
            {
                var workOrder = workOrders.FirstOrDefault(wo => wo.WorkOrderId == scheduledWoDto.WorkOrderId);
                if (workOrder is null) throw new ResourceNotFoundException(nameof(WorkOrder), scheduledWoDto.WorkOrderId);

                workOrder.UpdateDate(start: scheduledWoDto.StartTime,
                                     end: scheduledWoDto.EndTime,
                                     duration: scheduledWoDto.Duration,
                                     runtime: scheduledWoDto.RunTime,
                                     setuptime: scheduledWoDto.SetupTime);

                var equipmentWorkOrders = new List<EquipmentWorkOrder>();
                var equipment = await _equipmentRepository.GetAsync(scheduledWoDto.EquipmentId);
                if (equipment is null) throw new ResourceNotFoundException(nameof(Equipment), scheduledWoDto.EquipmentId);
                equipment.UpdateStatus(EStatus.Planned);
                var wc = await _equipmentRepository.GetWorkCenterByEquipmentId(equipment.HierarchyModel?.HierarchyModelId ?? "WC001");
                workOrder.UpdateWorkCenter(wc);
                var equipmentWorkOrder = new EquipmentWorkOrder(equipment, equipment.EquipmentClass.Name, workOrder);
                equipmentWorkOrders.Add(equipmentWorkOrder);

                var personWorkOrders = new List<PersonWorkOrder>();
                var person = await _personRepository.GetAsync(scheduledWoDto.PersonShift12);
                if (person is null) throw new ResourceNotFoundException(nameof(Person), scheduledWoDto.PersonShift12);
                person.UpdateStatus(EStatus.Planned);
                var personWorkOrder = new PersonWorkOrder(person, person.PersonnelClass.Name,workOrder, "Ca12");
                personWorkOrders.Add(personWorkOrder);
                person = await _personRepository.GetAsync(scheduledWoDto.PersonShift3);
                if (person is null) throw new ResourceNotFoundException(nameof(Person), scheduledWoDto.PersonShift3);
                person.UpdateStatus(EStatus.Planned);
                personWorkOrder = new PersonWorkOrder(person, person.PersonnelClass.Name, workOrder, "Ca3");
                personWorkOrders.Add(personWorkOrder);

                var materialWorkOrders = new List<MaterialWorkOrder>();
                var processSegment = await _processSegmentRepository.GetAsync(workOrder.OperationsSegment.SegmentId);
                if (processSegment is null) throw new ResourceNotFoundException(nameof(ProcessSegment), workOrder.OperationsSegment.SegmentId);

                foreach (var billItem in workOrder.OperationsSegment.OperationsDefinition.OperationsMaterialBill.Items)
                {
                    var materialDef = await _materialDefinitionRepository.GetAsync(billItem.MaterialDefinitionId);
                    if (materialDef is null) throw new ResourceNotFoundException(nameof(MaterialDefinition), billItem.MaterialDefinitionId);

                    

                    var materialSpecs = processSegment.MaterialSegmentSpecifications;
                    var spec = materialSpecs.FirstOrDefault(ms => ms.MaterialDefinition.ResourceId == materialDef.ResourceId);
                    if (spec is null)
                    {
                        continue;
                    }

                    var quantityValue = Convert.ToDecimal(billItem.Quantity.GetValue());
                    var quantity = quantityValue * workOrder.ProductionTarget;
                    var materialWorkOrder = new MaterialWorkOrder(materialDef, materialDef.MaterialClass.Name, new Value(quantity.ToString(),EValueType.Decimal), billItem.ValueUnitOfMeasure, workOrder);
                    materialWorkOrders.Add(materialWorkOrder);
                }

                // Lấy list MaterialDefinitionId trong bill
                var billMaterialIds = workOrder
                    .OperationsSegment
                    .OperationsDefinition
                    .OperationsMaterialBill
                    .Items
                    .Select(b => b.MaterialDefinitionId)
                    .ToHashSet();

                // Lấy list MaterialDefinitionId trong spec nhưng không có trong bill
                var materialsNotInBill = processSegment
                    .MaterialSegmentSpecifications
                    .Where(ms => !billMaterialIds.Contains(ms.MaterialDefinition.ResourceId))
                    .ToList();

                foreach (var specItem in materialsNotInBill)
                {
                    var materialDef = await _materialDefinitionRepository.GetAsync(specItem.MaterialDefinition.ResourceId);
                    if (materialDef is null) throw new ResourceNotFoundException(nameof(MaterialDefinition), specItem.MaterialDefinition.ResourceId);

                    decimal quantity = workOrder.ProductionTarget;

                    var mwo = new MaterialWorkOrder(
                        materialDef,
                        materialDef.MaterialClass.Name,
                        new Value(quantity.ToString(), EValueType.Decimal),
                        materialDef.PrimaryUnit, // hoặc null tùy trường
                        workOrder
                    );

                    materialWorkOrders.Add(mwo);
                }

                workOrder.UpdateSomeOrder(equipmentWorkOrders, personWorkOrders, materialWorkOrders);

                workOrder.UpdateStatus(EStatus.ReadyForProduction);
            }
            var check = await _workOrderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            if (!check) return false;

            var moIds = workOrders.Select(wo => wo.ManufacturingOrder.ManufacturingOrderId).Distinct().ToList();

            foreach (var moId in moIds)
            {
                await _manufacturingOrderRepository.UpdateDatesById(moId);
                await _manufacturingOrderRepository.UpdateStatusById(moId);
            }

            return await _manufacturingOrderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
