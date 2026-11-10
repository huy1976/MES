
namespace MesMicroservice.Application.Queries.MaterialDefinitions;

public class GetMaterialDefinitionIdsQueryHandler : IRequestHandler<GetMaterialDefinitionIdsQuery, IEnumerable<MaterialDefinitonIdViewModel>>
{
    private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

    public GetMaterialDefinitionIdsQueryHandler(IMaterialDefinitionRepository materialDefinitionRepository)
    {
        _materialDefinitionRepository = materialDefinitionRepository;
    }

    public async Task<IEnumerable<MaterialDefinitonIdViewModel>> Handle(GetMaterialDefinitionIdsQuery request, CancellationToken cancellationToken)
    {
        var materialDefinitions = await _materialDefinitionRepository.GetMaterialDefinitions();

        var materialDefinitionIds = materialDefinitions.Select(x => new MaterialDefinitonIdViewModel(x.ResourceId, x.Name)).ToList();

        return materialDefinitionIds;
    }
}
