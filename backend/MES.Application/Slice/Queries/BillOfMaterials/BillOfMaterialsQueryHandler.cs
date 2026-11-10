
namespace MesMicroservice.Application.Queries.BillOfMaterials;

public class BillOfMaterialsQueryHandler : IRequestHandler<BillOfMaterialsQuery, IEnumrable<BillOfMaterialViewModel>>
{
    private readonly IBillOfMaterialRepository _billOfMaterialRepository;
    private readonly IMapper _mapper;

    public BillOfMaterialsQueryHandler(IBillOfMaterialRepository billOfMaterialRepository, IMapper mapper)
    {
        _billOfMaterialRepository = billOfMaterialRepository;
        _mapper = mapper;
    }

    public async Task<IEnumrable<BillOfMaterialViewModel>> Handle(BillOfMaterialsQuery request, CancellationToken cancellationToken)
    {
        var queryable = await _billOfMaterialRepository.GetBillOfMaterials();

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

        var billOfMaterials = queryable.ToList();

        var queryResult = new IEnumrable<BillOfMaterial>(billOfMaterials, totalItems);

        return _mapper.Map<IEnumrable<BillOfMaterial>, IEnumrable<BillOfMaterialViewModel>>(queryResult);
    }
}
