namespace MesMicroservice.Application.Queries.WorkCenterOutputs;

public class WorkCenterOutputsByManuOrderQueryHandler : IRequestHandler<WorkCenterOutputsByManuOrderQuery, IEnumerable<WorkCenterOutputViewModel>>
{
    private readonly ApplicationDbContext _context;
    private readonly IManufacturingOrderRepository _manufacturingOrderRepository;

    public WorkCenterOutputsByManuOrderQueryHandler(ApplicationDbContext context, IManufacturingOrderRepository manufacturingOrderRepository)
    {
        _context = context;
        _manufacturingOrderRepository = manufacturingOrderRepository;
    }

    public async Task<IEnumerable<WorkCenterOutputViewModel>> Handle(WorkCenterOutputsByManuOrderQuery request, CancellationToken cancellationToken)
    {
        var manufacturingOrder = await _manufacturingOrderRepository.GetAsync(request.ManufacturingOrderId) ?? throw new ResourceNotFoundException(nameof(ManufacturingOrder), request.ManufacturingOrderId);

        return await _context.WorkCenterOutputs
            .Include(x => x.WorkCenter)
            .ThenInclude(x => x.Parent)
            .ThenInclude(x => x.Parent)
            .ThenInclude(x => x.Parent)
            .Include(x => x.MaterialDefinition)
            .Include(x => x.Unit)
            .Where(x => x.MaterialDefinition.ResourceId == manufacturingOrder.MaterialDefinition.ResourceId)
            .Select(x => new WorkCenterOutputViewModel(
                x.WorkCenter.AbsolutePath,
                x.MaterialDefinition.ResourceId,
                x.Output,
                x.Unit.UnitId))
            .ToListAsync();
    }
}
