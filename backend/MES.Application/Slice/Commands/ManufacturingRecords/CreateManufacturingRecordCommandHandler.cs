using Microsoft.AspNetCore.SignalR;

namespace MesMicroservice.Application.Commands.ManufacturingRecords;

public class CreateManufacturingRecordCommandHandler : IRequestHandler<CreateManufacturingRecordCommand, bool>
{
    private readonly IManufacturingRecordRepository _manufacturingRecordRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
    private readonly IHubContext<NotificationHub> _hubContext;

    public CreateManufacturingRecordCommandHandler(IManufacturingRecordRepository manufacturingRecordRepository, IEquipmentRepository equipmentRepository, IManufacturingOrderRepository manufacturingOrderRepository, IHubContext<NotificationHub> hubContext)
    {
        _manufacturingRecordRepository = manufacturingRecordRepository;
        _equipmentRepository = equipmentRepository;
        _manufacturingOrderRepository = manufacturingOrderRepository;
        _hubContext = hubContext;
    }

    public async Task<bool> Handle(CreateManufacturingRecordCommand request, CancellationToken cancellationToken)
    {
        var manufacturingOrder = await _manufacturingOrderRepository.GetAsync(request.ManufacturingOrderId) ?? throw new ResourceNotFoundException(nameof(ManufacturingOrder), request.ManufacturingOrderId);
        var workOrder = manufacturingOrder.WorkOrders.FirstOrDefault(x => x.WorkOrderId == request.WorkOrderId) ?? throw new ResourceNotFoundException(nameof(WorkOrder), request.WorkOrderId);

        var equipments = new List<Equipment>();
        foreach (var equipmentId in request.EquipmentIds)
        {
            var equipment = await _equipmentRepository.GetAsync(equipmentId) ?? throw new ResourceNotFoundException(nameof(Equipment), equipmentId);
            equipments.Add(equipment);
        }

        var manufacturingRecord = new ManufacturingRecord(
            workOrder,
            manufacturingOrder.MaterialDefinition,
            equipments,
            request.StartTime,
            request.EndTime,
            request.Output,
            request.Defects);

        _manufacturingRecordRepository.Add(manufacturingRecord);
        await _hubContext.Clients.All.SendAsync(
            "WorkOrderProgressUpdated",
            manufacturingOrder.ManufacturingOrderId,
            workOrder.WorkOrderId);
            //workOrder.ActualQuantity,
            //workOrder.Progress );

        return await _manufacturingRecordRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
