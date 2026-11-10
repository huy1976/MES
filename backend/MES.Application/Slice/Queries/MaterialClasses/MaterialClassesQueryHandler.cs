using MesMicroservice.Domain.AggregateModels.MaterialClassAggregate;

namespace MesMicroservice.Application.Queries.MaterialClasses;

public class MaterialClassesQueryHandler : IRequestHandler<MaterialClassesQuery, IEnumrable<MaterialClassViewModel>>
{
    private readonly IMaterialClassRepository _materialClassRepository;
    private readonly IMapper _mapper;

    public MaterialClassesQueryHandler(IMaterialClassRepository materialClassRepository, IMapper mapper)
    {
        _materialClassRepository = materialClassRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<MaterialClassViewModel>> Handle(MaterialClassesQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _materialClassRepository.GetMaterialClasses();
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

        var materialClasses = queryable.ToList();

        var queryResult = new IEnumrable<MaterialClass>(materialClasses, totalItems);

        return _mapper.Map<IEnumrable<MaterialClass>, IEnumrable<MaterialClassViewModel>>(queryResult);
    }
}
