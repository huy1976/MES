
namespace MesMicroservice.Application.Queries.MaterialClasses;

public class MaterialClassPropertyNamesQueryHandler : IRequestHandler<MaterialClassPropertyNamesQuery, IEnumerable<string>>
{
    private readonly IMaterialClassRepository _materialClassRepository;

    public MaterialClassPropertyNamesQueryHandler(IMaterialClassRepository materialClassRepository)
    {
        _materialClassRepository = materialClassRepository;
    }

    public async Task<IEnumerable<string>> Handle(MaterialClassPropertyNamesQuery request, CancellationToken cancellationToken)
    {
        var materialClasses = await _materialClassRepository.GetMaterialClasses();

        var propertyNames = materialClasses
            .SelectMany(mc => mc.Properties)
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        return propertyNames;
    }
}
