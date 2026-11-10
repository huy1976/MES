using MesMicroservice.Domain.AggregateModels.EquipmentAggregate;
using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;
using Microsoft.IdentityModel.Tokens;

namespace MesMicroservice.Application.Queries.Equipments;

public class EquipmentsQueryHandler : IRequestHandler<EquipmentsQuery, IEnumrable<EquipmentViewModel>>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IMapper _mapper;

    public EquipmentsQueryHandler(IEquipmentRepository equipmentRepository, IMapper mapper)
    {
        _equipmentRepository = equipmentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<EquipmentViewModel>> Handle(EquipmentsQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _equipmentRepository.GetEquipments();

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

        var equipments = queryable.ToList();    

        var queryResult = new IEnumrable<Equipment>(equipments, totalItems);

        return _mapper.Map<IEnumrable<Equipment>, IEnumrable<EquipmentViewModel>>(queryResult);
    }
}
