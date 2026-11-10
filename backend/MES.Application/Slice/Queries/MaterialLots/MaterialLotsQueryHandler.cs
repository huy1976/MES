using MesMicroservice.Domain.AggregateModels.MaterialLotAggregate;

namespace MesMicroservice.Application.Queries.MaterialLots;

public class MaterialLotsQueryHandler : IRequestHandler<MaterialLotsQuery, IEnumrable<MaterialLotViewModel>>
{
    private readonly IMaterialLotRepository _materialLotRepository;
    private readonly IMapper _mapper;

    public MaterialLotsQueryHandler(IMaterialLotRepository materialLotRepository, IMapper mapper)
    {
        _materialLotRepository = materialLotRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<MaterialLotViewModel>> Handle(MaterialLotsQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _materialLotRepository.GetMaterialLots();

        if (request.IdStartedWith is not null)
        {
            queryable = queryable.Where(x => x.ResourceId.StartsWith(request.IdStartedWith));
        }

        if (request.MaterialDefinitionId is not null)
        {
            queryable = queryable.Where(x => x.MaterialDefinition.ResourceId == request.MaterialDefinitionId);
        }

        if (request.Paginated)
        {
            queryable = queryable
                .OrderBy(x => x.ResourceId)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        int totalItems = queryable.Count();

        var materialLots = queryable.ToList();

        var queryResult = new IEnumrable<MaterialLot>(materialLots, totalItems);

        return _mapper.Map<IEnumrable<MaterialLot>, IEnumrable<MaterialLotViewModel>>(queryResult);
    }
}