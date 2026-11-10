using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;
using System.Linq;

namespace MesMicroservice.Application.Queries.Enterprises;

public class EnterprisesQueryHandler : IRequestHandler<EnterprisesQuery, IEnumrable<EnterpriseViewModel>>
{
    private readonly IEnterpriseRepository _enterpriseRepository;
    private readonly IMapper _mapper;

    public EnterprisesQueryHandler(IEnterpriseRepository enterpriseRepository, IMapper mapper)
    {
        _enterpriseRepository = enterpriseRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<EnterpriseViewModel>> Handle(EnterprisesQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _enterpriseRepository.GetEnterprises();

        if (request.IdStartedWith is not null)
        {
            queryable = queryable.Where(x => x.HierarchyModelId.StartsWith(request.IdStartedWith));
        }

        queryable = queryable
            .OrderBy(x => x.HierarchyModelId)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize);

        int totalItems = queryable.Count();

        var enterprises = queryable.ToList();

        var queryResult = new IEnumrable<Enterprise>(enterprises, totalItems);

        return _mapper.Map<IEnumrable<Enterprise>, IEnumrable<EnterpriseViewModel>>(queryResult);
    }
}
