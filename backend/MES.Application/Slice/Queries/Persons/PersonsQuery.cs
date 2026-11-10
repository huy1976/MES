namespace MesMicroservice.Application.Queries.Persons;

public class PersonsQuery : PaginatedQuery, IRequest<IEnumrable<PersonViewModel>>
{ 
    public string? IdStartedWith { get; set; }
}
