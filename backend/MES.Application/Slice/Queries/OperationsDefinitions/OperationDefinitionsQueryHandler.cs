
namespace MesMicroservice.Application.Queries.OperationsDefinitions;

public class OperationDefinitionsQueryHandler : IRequestHandler<OperationDefinitionsQuery, IEnumrable<OperationsDefinitionViewModel>>
{
    private readonly IOperationsDefinitionRepository _operationsDefinitionRepository;
    private readonly IMapper _mapper;

    public OperationDefinitionsQueryHandler(IOperationsDefinitionRepository operationsDefinitionRepository, IMapper mapper)
    {
        _operationsDefinitionRepository = operationsDefinitionRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<OperationsDefinitionViewModel>> Handle(OperationDefinitionsQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _operationsDefinitionRepository.GetAllAsync();

        if (request.IdStartedWith is not null)
        {
            queryable = queryable.Where(x => x.ResourceId.StartsWith(request.IdStartedWith));
        }

        if (request.MaterialId is not null)
        {
            queryable = queryable.Where(x => x.MaterialDefinition.ResourceId == request.MaterialId);
        }

        if (request.Paginated)
        {
            queryable = queryable
                .OrderBy(x => x.ResourceId)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        int totalItems = queryable.Count();

        var operationsDefinitions = queryable.ToList();

        var queryResult = new IEnumrable<OperationsDefinition>(operationsDefinitions, totalItems);

        return _mapper.Map<IEnumrable<OperationsDefinition>, IEnumrable<OperationsDefinitionViewModel>>(queryResult);
    }
}
