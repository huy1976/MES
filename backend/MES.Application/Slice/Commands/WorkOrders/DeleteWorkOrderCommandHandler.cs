

namespace MesMicroservice.Application.Commands.WorkOrders;

public class DeleteWorkOrderCommandHandler : IRequestHandler<DeleteWorkOrderCommand, bool>
{
    private IWorkOrderRepository _workOrderRepository;
    private IManufacturingOrderRepository _manufacturingOrderRepository;

    public DeleteWorkOrderCommandHandler(IWorkOrderRepository workOrderRepository, IManufacturingOrderRepository manufacturingOrderRepository)
    {
        _workOrderRepository = workOrderRepository;
        _manufacturingOrderRepository = manufacturingOrderRepository;
    }

    public async Task<bool> Handle(DeleteWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var manufacturingOrder = await _manufacturingOrderRepository.GetAsync(request.ManufacturingOrderId) ?? throw new ResourceNotFoundException(nameof(ManufacturingOrder), request.ManufacturingOrderId);

        await _workOrderRepository.DeleteAsync(manufacturingOrder.Id, request.WorkOrderId);
        return await _workOrderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}

