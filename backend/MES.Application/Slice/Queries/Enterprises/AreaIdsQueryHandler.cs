
namespace MesMicroservice.Application.Queries.Enterprises;

public class AreaIdsQueryHandler : IRequestHandler<AreaIdsQuery, IEnumerable<string>>
{
    private readonly IAreaRepository _areaRepository;

    public AreaIdsQueryHandler(IAreaRepository areaRepository)
    {
        _areaRepository = areaRepository;
    }

    public async Task<IEnumerable<string>> Handle(AreaIdsQuery request, CancellationToken cancellationToken)
    {
        var areas = await _areaRepository.GetAreas();

        return areas.Select(x => x.HierarchyModelId).ToList();
    }
}
