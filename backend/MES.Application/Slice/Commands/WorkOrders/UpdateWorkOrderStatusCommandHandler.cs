using MesMicroservice.Domain.AggregateModels.WorkOrderAggregate;

namespace MesMicroservice.Application.Commands.WorkOrders
{
    public class UpdateWorkOrderStatusCommandHandler : IRequestHandler<UpdateWorkOrderStatusCommand, bool>
    {
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IPersonRepository _personRepository;
        public UpdateWorkOrderStatusCommandHandler(IManufacturingOrderRepository manufacturingOrderRepository, IEquipmentRepository equipmentRepository, IPersonRepository personRepository)
        {
            _manufacturingOrderRepository = manufacturingOrderRepository;
            _equipmentRepository = equipmentRepository;
            _personRepository = personRepository;
        }

        public async Task<bool> Handle(UpdateWorkOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var manufacturingOrder = await _manufacturingOrderRepository.GetById(request.ManufacturingOrderId) ?? throw new ResourceNotFoundException(nameof(ManufacturingOrder), request.ManufacturingOrderId);
            var workOrder = manufacturingOrder.WorkOrders.FirstOrDefault(wo => wo.WorkOrderId == request.WorkOrderId) ?? throw new ResourceNotFoundException(nameof(WorkOrder), request.WorkOrderId);
            workOrder.UpdateStatus(request.WorkOrderStatus);
            var equipments = await _equipmentRepository.GetAllAsync();
            var persons = await _personRepository.GetAll();
            if (request.WorkOrderStatus == EStatus.Completed)
            {
                foreach (var equipmentWorkOrder in workOrder.EquipmentWorkOrders)
                {
                    var equipment = equipments.Find(e => e.ResourceId == equipmentWorkOrder.Equipment.ResourceId);
                    if (equipment is null) continue;
                    equipment.UpdateStatus(EStatus.Available);
                }
                foreach (var personWorkOrder in workOrder.PersonWorkOrders)
                {
                    var person = persons.Find(e => e.ResourceId == personWorkOrder.Person.ResourceId);
                    if (person is null) continue;
                    person.UpdateStatus(EStatus.Available);
                }
                workOrder.UpdateActuallyTime(workOrder.ActuallyStartTime, DateTime.Now);
            }
            else if (request.WorkOrderStatus == EStatus.InProduction)
            {
                foreach (var equipmentWorkOrder in workOrder.EquipmentWorkOrders)
                {
                    var equipment = equipments.Find(e => e.ResourceId == equipmentWorkOrder.Equipment.ResourceId);
                    if (equipment is null) continue;
                    equipment.UpdateStatus(EStatus.Running);
                }
                foreach (var personWorkOrder in workOrder.PersonWorkOrders)
                {
                    var person = persons.Find(e => e.ResourceId == personWorkOrder.Person.ResourceId);
                    if (person is null) continue;
                    person.UpdateStatus(EStatus.Available);
                }
                workOrder.UpdateActuallyTime(DateTime.Now, workOrder.ActuallyEndTime);
            }
            await _manufacturingOrderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            await _manufacturingOrderRepository.UpdateStatusById(request.ManufacturingOrderId);

            return await _manufacturingOrderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }

    }
}
