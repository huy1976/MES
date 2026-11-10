
namespace MesMicroservice.Application.Queries.SegmentModels;

public class MaterialSegmentSpecificationPropertyNamesQueryHandler : IRequestHandler<MaterialSegmentSpecificationPropertyNamesQuery, IEnumerable<string>>
{
    private readonly IMaterialSegmentSpecificationRepository _materialSegmentSpecificationRepository;
    public MaterialSegmentSpecificationPropertyNamesQueryHandler(IMaterialSegmentSpecificationRepository materialSegmentSpecificationRepository)
    {
        _materialSegmentSpecificationRepository = materialSegmentSpecificationRepository;
    }
    public async Task<IEnumerable<string>> Handle(MaterialSegmentSpecificationPropertyNamesQuery request, CancellationToken cancellationToken)
    {
        var materialSegmentSpecifications = await _materialSegmentSpecificationRepository.GetMaterialSegmentSpecifications();

        var propertyNames = materialSegmentSpecifications
            .SelectMany(mss => mss.Properties)
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        return propertyNames;
    }
}
