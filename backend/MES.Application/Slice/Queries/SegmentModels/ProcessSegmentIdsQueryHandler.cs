
namespace MesMicroservice.Application.Queries.SegmentModels;

public class ProcessSegmentIdsQueryHandler : IRequestHandler<ProcessSegmentIdsQuery, IEnumerable<string>>
{
    private readonly IProcessSegmentRepository _processSegmentRepository;
    public ProcessSegmentIdsQueryHandler(IProcessSegmentRepository processSegmentRepository)
    {
        _processSegmentRepository = processSegmentRepository;
    }

    public async Task<IEnumerable<string>> Handle(ProcessSegmentIdsQuery request, CancellationToken cancellationToken)
    {
       var processSegments = await _processSegmentRepository.GetProcessSegments();

       return processSegments.Select(x => x.ResourceId);
    }
}
