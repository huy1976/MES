using MesMicroservice.Domain.AggregateModels.MaterialDefinitionAggregate;

namespace MesMicroservice.Application.Queries.MaterialDefinitions;

public class MaterialDefinitionsQueryHandler : IRequestHandler<MaterialDefinitionsQuery, IEnumrable<MaterialDefinitionViewModel>>
{
    private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
    private readonly IMapper _mapper;

    public MaterialDefinitionsQueryHandler(IMaterialDefinitionRepository materialDefinitionRepository, IMapper mapper)
    {
        _materialDefinitionRepository = materialDefinitionRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<MaterialDefinitionViewModel>> Handle(MaterialDefinitionsQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _materialDefinitionRepository.GetMaterialDefinitions();

        if (request.IdStartedWith is not null)
        {
            queryable = queryable.Where(x => x.ResourceId.StartsWith(request.IdStartedWith));
        }

        if (request.Paginated)
        {
            queryable = queryable
                .OrderBy(x => x.ResourceId)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        int totalItems = queryable.Count();

        var materialDefintions =  queryable.ToList();

        var queryResult = new IEnumrable<MaterialDefinition>(materialDefintions, totalItems);

        return _mapper.Map<IEnumrable<MaterialDefinition>, IEnumrable<MaterialDefinitionViewModel>>(queryResult);
    }
}
