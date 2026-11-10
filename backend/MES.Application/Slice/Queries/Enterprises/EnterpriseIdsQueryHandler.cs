
namespace MesMicroservice.Application.Queries.Enterprises;

public class EnterpriseIdsQueryHandler : IRequestHandler<EnterpriseIdsQuery, IEnumerable<string>>
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public EnterpriseIdsQueryHandler(IEnterpriseRepository enterpriseRepository)
    {
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<IEnumerable<string>> Handle(EnterpriseIdsQuery request, CancellationToken cancellationToken)
    {
        var enterprises = await _enterpriseRepository.GetEnterprises();

        return enterprises.Select(x => x.HierarchyModelId).ToList();
    }
}
