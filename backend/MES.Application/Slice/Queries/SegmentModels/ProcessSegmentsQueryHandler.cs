
namespace MesMicroservice.Application.Queries.SegmentModels;

public class ProcessSegmentsQueryHandler : IRequestHandler<ProcessSegmentsQuery, IEnumrable<ProcessSegmentViewModel>>
{
    private readonly IProcessSegmentRepository _processSegmentRepository;
    private readonly IMapper _mapper;

    public ProcessSegmentsQueryHandler(IProcessSegmentRepository processSegmentRepository, IMapper mapper)
    {
        _processSegmentRepository = processSegmentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<ProcessSegmentViewModel>> Handle(ProcessSegmentsQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _processSegmentRepository.GetProcessSegments();
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

        var processSegments = queryable.ToList();

        var queryResult = new IEnumrable<ProcessSegment>(processSegments, totalItems);

        return _mapper.Map<IEnumrable<ProcessSegment>, IEnumrable<ProcessSegmentViewModel>>(queryResult);
    }
}
