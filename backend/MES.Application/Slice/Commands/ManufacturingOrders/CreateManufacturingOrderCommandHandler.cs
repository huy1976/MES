namespace MesMicroservice.Application.Commands.ManufacturingOrders;

public class CreateManufacturingOrderCommandHandler : IRequestHandler<CreateManufacturingOrderCommand, bool>
{
    private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
    private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
    private readonly IOperationsDefinitionRepository _operationsDefinitionRepository;
    private readonly IQualityStandardRepository _qualityStandardRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IBusinessPartnerRepository _buinessPartnerRepository;
    private readonly DueDateBuffer _dueDateBuffer;

    public CreateManufacturingOrderCommandHandler(IManufacturingOrderRepository manufacturingOrderRepository, IMaterialDefinitionRepository materialDefinitionRepository, IOperationsDefinitionRepository operationsDefinitionRepository, IQualityStandardRepository qualityStandardRepository, IPersonRepository personRepository, IBusinessPartnerRepository buinessPartnerRepository, DueDateBuffer dueDateBuffer)
    {
        _manufacturingOrderRepository = manufacturingOrderRepository;
        _materialDefinitionRepository = materialDefinitionRepository;
        _operationsDefinitionRepository = operationsDefinitionRepository;
        _qualityStandardRepository = qualityStandardRepository;
        _personRepository = personRepository;
        _buinessPartnerRepository = buinessPartnerRepository;
        _dueDateBuffer = dueDateBuffer;
    }

    public async Task<bool> Handle(CreateManufacturingOrderCommand request, CancellationToken cancellationToken)
    {
        var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId) ?? throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialDefinitionId);
        var operationsDefinition = await _operationsDefinitionRepository.GetAsync(request.OperationsDefinitionId) ?? throw new ResourceNotFoundException(nameof(OperationsDefinition), request.OperationsDefinitionId);
        var qualityStandard = await _qualityStandardRepository.GetAsync(request.QualityStandardId) ?? throw new ResourceNotFoundException(nameof(QualityStandard), request.QualityStandardId);
        var createdBy = await _personRepository.GetAsync(request.CreatedById) ?? throw new ResourceNotFoundException(nameof(Person), request.CreatedById);
        var customer = await _buinessPartnerRepository.GetAsync(request.CustomerId) ?? throw new ResourceNotFoundException(nameof(BusinessPartner), request.CustomerId);

        var checkManufacturingOrderExists = await _manufacturingOrderRepository.ExistsAsync(request.ManufacturingOrderId);
        if (checkManufacturingOrderExists)
        {
            throw new ResourceAlreadyExistsException(nameof(ManufacturingOrder), request.ManufacturingOrderId);
        }
        var manufacturingOrder = new ManufacturingOrder(manufacturingOrderId: request.ManufacturingOrderId, 
                                                        materialDefinition: materialDefinition, 
                                                        quantity: request.Quantity, 
                                                        unit: request.Unit,
                                                        availableDate: DateTime.MinValue,
                                                        dueDate: request.DueDate,
                                                        planedStartDate: DateTime.MinValue,
                                                        planedEnDate: DateTime.MinValue,
                                                        customer: customer,
                                                        operationsDefinition: operationsDefinition,
                                                        qualityStandard: qualityStandard,
                                                        status: EStatus.Pending,
                                                        createDate: DateTime.UtcNow,
                                                        createdBy: createdBy,
                                                        priority: Math.Clamp(request.Priority, 1, 100),
                                                        workOrders: new List<WorkOrder>());

        await _manufacturingOrderRepository.Add(manufacturingOrder);

        return await _manufacturingOrderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

    }

    //public async Task<bool> Handle(CreateManufacturingOrderCommand request, CancellationToken cancellationToken)
    //{
    //    var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId) ?? throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialDefinitionId);
    //    var manufacturingOrder = new ManufacturingOrder(request.ManufacturingOrderId, materialDefinition, request.Quantity, request.Unit, request.DueDate, request.AvailableDate, request.Priority);

    //    await CreateWorkOrders(manufacturingOrder, materialDefinition.Operations);
    //    await _manufacturingOrderRepository.Add(manufacturingOrder);

    //    return await _manufacturingOrderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    //}

    //private async Task CreateWorkOrders(ManufacturingOrder manufacturingOrder,List<Operation> operations)
    //{
    //    var workOrders = manufacturingOrder.WorkOrders;
    //    workOrders.AddRange(operations.Select(x => new WorkOrder(x.OperationId)));

    //    foreach (var operation in operations)
    //    {
    //        var workOrder = workOrders.Find(x => x.WorkOrderId == operation.OperationId);
    //        var prerequisiteOperations = workOrders
    //            .Where(x => operation.PrerequisiteOperation.Exists(op => op.OperationId == x.WorkOrderId))
    //            .ToList();

    //        workOrder?.SetPrerequisiteOperations(prerequisiteOperations);
    //        workOrder?.SetDuration(TimeSpan.FromHours(3600));
    //        workOrder?.SetProductionTarget(manufacturingOrder.Quantity);
    //    }

    //    var prerequisiteWorkOrders = workOrders.SelectMany(x => x.PrerequisiteOperations);
    //    var lastestWorkOrders = workOrders.Except(prerequisiteWorkOrders);

    //    workOrders.ForEach(x => _dueDateBuffer.Add(x.WorkOrderId));

    //    foreach (var workOrder in lastestWorkOrders)
    //    {
    //        await SetWorkOrderTime(workOrder, manufacturingOrder.DueDate);
    //    }

    //    workOrders.ForEach(x => _dueDateBuffer.Remove(x.WorkOrderId));
    //}

    //private async Task SetWorkOrderTime(WorkOrder workOrder, DateTime dueDate)
    //{
    //    DateTime prereqDueDate = _dueDateBuffer.GetPrereqDueDate(workOrder.WorkOrderId);

    //    if (dueDate < prereqDueDate)
    //    {
    //        workOrder.SetDueDate(dueDate);
    //        _dueDateBuffer.Update(workOrder.WorkOrderId, dueDate);
    //    }

    //    if (workOrder.PrerequisiteOperations.Count > 0)
    //    {
    //        foreach (var prereqWorkOrder in workOrder.PrerequisiteOperations)
    //        {
    //            await SetWorkOrderTime(prereqWorkOrder, workOrder.DueDate - workOrder.Duration);
    //        }
    //    }
    //}


}
