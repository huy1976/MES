using MesMicroservice.Domain.AggregateModels.EquipmentClassAggregate;

namespace MesMicroservice.Application.Queries.EquipmentClasses;

public class EquipmentClassesQueryHandler: IRequestHandler<EquipmentClassesQuery, IEnumrable<EquipmentClassViewModel>>
{
    private readonly IEquipmentClassRepository _equipmentClassRepository;
    private readonly IMapper _mapper;

    public EquipmentClassesQueryHandler(IEquipmentClassRepository equipmentClassRepository, IMapper mapper)
    {
        _equipmentClassRepository = equipmentClassRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<EquipmentClassViewModel>> Handle(EquipmentClassesQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _equipmentClassRepository.GetEquipmentClasses();

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

        var equipmentClasses = queryable.ToList();

        var queryResult = new IEnumrable<EquipmentClass>(equipmentClasses, totalItems);

        return _mapper.Map<IEnumrable<EquipmentClass>, IEnumrable<EquipmentClassViewModel>>(queryResult);
    }
}
