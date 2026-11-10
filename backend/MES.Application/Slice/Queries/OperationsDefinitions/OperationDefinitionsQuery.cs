

namespace MesMicroservice.Application.Queries.OperationsDefinitions;

public class OperationDefinitionsQuery : PaginatedQuery, IRequest<IEnumrable<OperationsDefinitionViewModel>>
{
    public string? IdStartedWith { get; set; }
    public string? MaterialId { get; set; }

}
