namespace MesMicroservice.Application.Queries.MaterialDefinitions;

public class MaterialDefinitionsQuery : PaginatedQuery, IRequest<IEnumrable<MaterialDefinitionViewModel>>
{
    public string? IdStartedWith { get; set; }
}
