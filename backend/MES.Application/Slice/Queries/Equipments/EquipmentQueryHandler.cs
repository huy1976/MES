using MesMicroservice.Domain.AggregateModels.EquipmentAggregate;
using MesMicroservice.Domain.AggregateModels.HierarchyModelAggregate;

namespace MesMicroservice.Application.Queries.Equipments;

public class EquipmentQueryHandler : IRequestHandler<EquipmentQuery, EquipmentViewModel?>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IMapper _mapper;

    public EquipmentQueryHandler(IEquipmentRepository equipmentRepository, IMapper mapper)
    {
        _equipmentRepository = equipmentRepository;
        _mapper = mapper;
    }

    public async Task<EquipmentViewModel?> Handle(EquipmentQuery request, CancellationToken cancellationToken)
    {
        var equipment = await _equipmentRepository.GetAsync(request.EquipmentId);

        return _mapper.Map<Equipment?, EquipmentViewModel?>(equipment);
    }
}
