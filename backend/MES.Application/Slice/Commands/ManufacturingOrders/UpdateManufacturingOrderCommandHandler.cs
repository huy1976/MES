namespace MesMicroservice.Application.Commands.ManufacturingOrders;

public class UpdateManufacturingOrderCommandHandler : IRequestHandler<UpdateManufacturingOrderCommand, bool>
{
    private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
    private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

    public UpdateManufacturingOrderCommandHandler(IManufacturingOrderRepository manufacturingOrderRepository, IMaterialDefinitionRepository materialDefinitionRepository)
    {
        _manufacturingOrderRepository = manufacturingOrderRepository;
        _materialDefinitionRepository = materialDefinitionRepository;
    }

    public async Task<bool> Handle(UpdateManufacturingOrderCommand request, CancellationToken cancellationToken)
    {
        var manufacturingOrder = await _manufacturingOrderRepository.GetAsync(request.ManufacturingOrderId) ?? throw new ResourceNotFoundException(nameof(ManufacturingOrder), request.ManufacturingOrderId);
        var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId) ?? throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialDefinitionId);

        manufacturingOrder.Update(materialDefinition, request.Quantity, request.Unit, request.DueDate, request.AvailableDate, request.Priority);
        _manufacturingOrderRepository.Update(manufacturingOrder);

        return await _manufacturingOrderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
