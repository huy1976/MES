namespace MesMicroservice.Application.Queries.PersonnelClasses;

public class PersonnelClassesQuery : PaginatedQuery, IRequest<IEnumrable<PersonnelClassViewModel>>
{
    public string? IdStartedWith { get; set; }
}
