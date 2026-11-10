namespace MesMicroservice.Application.Queries.WorkCenterOutputs;

public class WorkCenterOutputsQueryHandler : IRequestHandler<WorkCenterOutputsQuery, IEnumerable<WorkCenterOutputViewModel>>
{
    private readonly ApplicationDbContext _context;
    private readonly IEnterpriseRepository _enterpriseRepository;

    public WorkCenterOutputsQueryHandler(ApplicationDbContext context, IEnterpriseRepository enterpriseRepository)
    {
        _context = context;
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<IEnumerable<WorkCenterOutputViewModel>> Handle(WorkCenterOutputsQuery request, CancellationToken cancellationToken)
    {
        var workCenter = await GetWorkCenterByAbsolutePath(request.AbsolutePath);

        return await _context.WorkCenterOutputs
            .Include(x => x.WorkCenter)
            .ThenInclude(x => x.Parent)
            .ThenInclude(x => x.Parent)
            .ThenInclude(x => x.Parent)
            .Include(x => x.MaterialDefinition)
            .Include(x => x.Unit)
            .Where(x => x.WorkCenter == workCenter)
            .Select(x => new WorkCenterOutputViewModel(
                x.WorkCenter.AbsolutePath,
                x.MaterialDefinition.ResourceId,
                x.Output,
                x.Unit.UnitId))
            .ToListAsync();
    }

    private async Task<WorkCenter> GetWorkCenterByAbsolutePath(string absolutePath)
    {
        var hierarchyModelIds = absolutePath.Split('/');
        var enterprise = await _enterpriseRepository.GetAsync(hierarchyModelIds[0]) ?? throw new ResourceNotFoundException(nameof(Enterprise), hierarchyModelIds[0]);
        var workCenter = enterprise.Sites
            .SelectMany(x => x.Areas)
            .SelectMany(x => x.WorkCenters)
            .FirstOrDefault(x => x.AbsolutePath == absolutePath) ?? throw new ResourceNotFoundException(nameof(WorkCenter), hierarchyModelIds[3]);

        return workCenter;
    }
}
