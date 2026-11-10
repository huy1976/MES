namespace MesMicroservice.Application.Commands.ManufacturingOrders;

public class DeleteManufacturingOrderCommandHandler : IRequestHandler<DeleteManufacturingOrderCommand, bool>
{
    private readonly IManufacturingOrderRepository _manufacturingOrderRepository;

    public DeleteManufacturingOrderCommandHandler(IManufacturingOrderRepository manufacturingOrderRepository)
    {
        _manufacturingOrderRepository = manufacturingOrderRepository;
    }

    public async Task<bool> Handle(DeleteManufacturingOrderCommand request, CancellationToken cancellationToken)
    {
        await _manufacturingOrderRepository.DeleteAsync(request.ManufacturingOrderId);

        return await _manufacturingOrderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
