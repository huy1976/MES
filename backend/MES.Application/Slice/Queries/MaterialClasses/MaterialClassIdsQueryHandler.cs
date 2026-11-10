

namespace MesMicroservice.Application.Queries.MaterialClasses;

public class MaterialClassIdsQueryHandler : IRequestHandler<MaterialClassIdsQuery, IEnumerable<MaterialClassIdViewModel>>
{
    private readonly IMaterialClassRepository _materialClassRepository;

    public MaterialClassIdsQueryHandler(IMaterialClassRepository materialClassRepository)
    {
        _materialClassRepository = materialClassRepository;
    }

    public async Task<IEnumerable<MaterialClassIdViewModel>> Handle(MaterialClassIdsQuery request, CancellationToken cancellationToken)
    {
        var materialClasses = await _materialClassRepository.GetMaterialClasses();

        return materialClasses.Select(x => new MaterialClassIdViewModel(x.ResourceId, x.Name));
    }
}
