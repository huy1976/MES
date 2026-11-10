

namespace MesMicroservice.Application.Queries.BillOfMaterials;

public class BillOfMaterialQueryHandler : IRequestHandler<BillOfMaterialQuery, BillOfMaterialViewModel?>
{
    private readonly IBillOfMaterialRepository _billOfMaterialRepository;
    private readonly IMapper _mapper;

    public BillOfMaterialQueryHandler(IBillOfMaterialRepository billOfMaterialRepository, IMapper mapper)
    {
        _billOfMaterialRepository = billOfMaterialRepository;
        _mapper = mapper;
    }

    public async Task<BillOfMaterialViewModel?> Handle(BillOfMaterialQuery request, CancellationToken cancellationToken)
    {
        var billOfMaterial = await _billOfMaterialRepository.GetAsync(request.BillOfMaterialId);

        return _mapper.Map<BillOfMaterial?, BillOfMaterialViewModel?>(billOfMaterial);
    }
}
