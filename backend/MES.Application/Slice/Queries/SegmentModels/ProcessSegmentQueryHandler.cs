
namespace MesMicroservice.Application.Queries.SegmentModels;

public class ProcessSegmentQueryHandler : IRequestHandler<ProcessSegmentQuery, ProcessSegmentViewModel?>
{
    private readonly IProcessSegmentRepository _processSegmentRepository;
    private readonly IMapper _mapper;

    public ProcessSegmentQueryHandler(IProcessSegmentRepository processSegmentRepository, IMapper mapper)
    {
        _processSegmentRepository = processSegmentRepository;
        _mapper = mapper;
    }

    public async Task<ProcessSegmentViewModel?> Handle(ProcessSegmentQuery request, CancellationToken cancellationToken)
    {
        var processSegment = await _processSegmentRepository.GetAsync(request.ProcessSegmentId);

        return _mapper.Map<ProcessSegment?, ProcessSegmentViewModel?>(processSegment);
    }
}
