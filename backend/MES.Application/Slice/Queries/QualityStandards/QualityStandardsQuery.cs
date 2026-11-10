namespace MesMicroservice.Application.Queries.QualityStandards;

public class QualityStandardsQuery : PaginatedQuery, IRequest<IEnumrable<QualityStandardViewModel>>
{
    public string? IdStartedWith { get; set; }
}
