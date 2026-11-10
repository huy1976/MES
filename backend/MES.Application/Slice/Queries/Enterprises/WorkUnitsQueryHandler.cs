using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Application.Queries.Enterprises;

public class WorkUnitsQueryHandler : IRequestHandler<WorkUnitsQuery, IEnumrable<WorkUnitViewModel>>
{
    private readonly IEnterpriseRepository _enterpriseRepository;
    private readonly IMapper _mapper;

    public WorkUnitsQueryHandler(IEnterpriseRepository enterpriseRepository, IMapper mapper)
    {
        _enterpriseRepository = enterpriseRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<WorkUnitViewModel>> Handle(WorkUnitsQuery request, CancellationToken cancellationToken)
    {
        var enterprises = await _enterpriseRepository.GetEnterprises();

        var workUnits = enterprises.SelectMany(x => x.Sites)
                .SelectMany(x => x.Areas)
                .SelectMany(x => x.WorkCenters)
                .SelectMany(x => x.WorkUnits);

        if (request.IdStartedWith is not null)
        {
            workUnits = workUnits.Where(x => x.HierarchyModelId.StartsWith(request.IdStartedWith));
        }

        workUnits = workUnits
                .OrderBy(x => x.HierarchyModelId)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);

        int totalItems = workUnits.Count();

        var queryResult = new IEnumrable<WorkUnit>(workUnits, totalItems);

        return _mapper.Map<IEnumrable<WorkUnit>, IEnumrable<WorkUnitViewModel>>(queryResult);
    }
}
