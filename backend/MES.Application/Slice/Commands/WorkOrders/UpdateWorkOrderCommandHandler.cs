namespace MesMicroservice.Application.Commands.WorkOrders;

public class UpdateWorkOrderCommandHandler : IRequestHandler<UpdateWorkOrderCommand, bool>
{
    private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IEnterpriseRepository _enterpriseRepository;

    public UpdateWorkOrderCommandHandler(IManufacturingOrderRepository manufacturingOrderRepository, IWorkOrderRepository workOrderRepository, IEnterpriseRepository enterpriseRepository)
    {
        _manufacturingOrderRepository = manufacturingOrderRepository;
        _workOrderRepository = workOrderRepository;
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(UpdateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var manufacturingOrder = await _manufacturingOrderRepository.GetAsync(request.ManufacturingOrderId) ?? throw new ResourceNotFoundException(nameof(ManufacturingOrder), request.ManufacturingOrderId);
        var workOrder = await _workOrderRepository.GetAsync(manufacturingOrder.Id, request.WorkOrderId) ?? throw new ResourceNotFoundException(nameof(WorkOrder), request.WorkOrderId);
        var workCenter = await GetWorkCenterByAbsolutePath(request.WorkCenter);

        workOrder.Update((request.EndTime - request.StartTime).TotalHours, request.StartTime, request.EndTime, request.ActuallyStartTime, request.ActuallyEndTime, request.WorkOrderStatus, workCenter);

        _workOrderRepository.Update(workOrder);
        return await _workOrderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }

    private async Task<WorkCenter?> GetWorkCenterByAbsolutePath(string absolutePath)
    {
        if (string.IsNullOrEmpty(absolutePath))
        {
            return null;
        }

        var hierarchyModelIds = absolutePath.Split('/');
        var enterprise = await _enterpriseRepository.GetAsync(hierarchyModelIds[0]) ?? throw new ResourceNotFoundException(nameof(Enterprise), hierarchyModelIds[0]);
        var workCenter = enterprise.Sites
            .SelectMany(x => x.Areas)
            .SelectMany(x => x.WorkCenters)
            .FirstOrDefault(x => x.AbsolutePath == absolutePath) ?? throw new ResourceNotFoundException(nameof(WorkCenter), hierarchyModelIds[3]);

        return workCenter;
    }
}
