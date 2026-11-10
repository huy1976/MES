namespace MesMicroservice.Application.Queries.MaterialClasses;

public class MaterialClassesQuery : PaginatedQuery, IRequest<IEnumrable<MaterialClassViewModel>>
{
    public string? IdStartedWith { get; set; }
}
