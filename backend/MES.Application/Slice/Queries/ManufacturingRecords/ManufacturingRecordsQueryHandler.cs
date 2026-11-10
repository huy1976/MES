using MesMicroservice.Domain.AggregateModels.ManufacucturingRecordAggregate;

namespace MesMicroservice.Application.Queries.ManufacturingRecords;

public class ManufacturingRecordsQueryHandler : IRequestHandler<ManufacturingRecordsQuery, IEnumrable<ManufacturingRecordViewModel>>
{
    private readonly IManufacturingRecordRepository _manufacturingRecordRepository;
    private readonly IMapper _mapper;

    public ManufacturingRecordsQueryHandler(IManufacturingRecordRepository manufacturingRecordRepository, IMapper mapper)
    {
        _manufacturingRecordRepository = manufacturingRecordRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<ManufacturingRecordViewModel>> Handle(ManufacturingRecordsQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _manufacturingRecordRepository.GetAllAsync();
        if (request.StartTime is not null)
        {
            queryable = queryable.Where(x => x.StartTime > request.StartTime);
        }

        if (request.EndTime is not null)
        {
            queryable = queryable.Where(x => x.EndTime < request.EndTime);
        }

        if (request.EquipmentId is not null)
        {
            queryable = queryable.Where(x => x.Equipments.Any(x => x.ResourceId == request.EquipmentId));
        }

        int totalItems = queryable.Count();

        if (request.Paginated)
        {
            queryable = queryable
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        var manufacturingRecords = queryable.ToList();
        var queryResult = new IEnumrable<ManufacturingRecord>(manufacturingRecords, totalItems);

        return _mapper.Map<IEnumrable<ManufacturingRecord>, IEnumrable<ManufacturingRecordViewModel>>(queryResult);
    }
}
