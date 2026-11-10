
namespace MesMicroservice.Application.Queries.QualityStandards;

public class QualityStandardsQueryHandler : IRequestHandler<QualityStandardsQuery, IEnumrable<QualityStandardViewModel>>
{
    private readonly IQualityStandardRepository _qualityStandardRepository;
    private readonly IMapper _mapper;

    public QualityStandardsQueryHandler(IQualityStandardRepository qualityStandardRepository, IMapper mapper)
    {
        _qualityStandardRepository = qualityStandardRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<QualityStandardViewModel>> Handle(QualityStandardsQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _qualityStandardRepository.GetQualityStandards();
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

        var qualityStandards = queryable.ToList();

        var queryResult = new IEnumrable<QualityStandard>(qualityStandards, totalItems);

        return _mapper.Map<IEnumrable<QualityStandard>, IEnumrable<QualityStandardViewModel>>(queryResult);
    }
}
