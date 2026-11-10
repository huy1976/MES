
namespace MesMicroservice.Application.Queries.BillOfMaterials;

public class BillOfMaterialIdsQueryHandler : IRequestHandler<BillOfMaterialIdsQuery, IEnumerable<string>>
{
    private readonly IBillOfMaterialRepository _billOfMaterialRepository;
    public BillOfMaterialIdsQueryHandler(IBillOfMaterialRepository billOfMaterialRepository)
    {
        _billOfMaterialRepository = billOfMaterialRepository;
    }
    public async Task<IEnumerable<string>> Handle(BillOfMaterialIdsQuery request, CancellationToken cancellationToken)
    {
        var billOfMaterials = await _billOfMaterialRepository.GetBillOfMaterials();
        return billOfMaterials.Select(x => x.ResourceId);
    }
}
