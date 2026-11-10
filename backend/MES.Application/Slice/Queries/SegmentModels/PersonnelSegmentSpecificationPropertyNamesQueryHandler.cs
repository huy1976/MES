
namespace MesMicroservice.Application.Queries.SegmentModels;

public class PersonnelSegmentSpecificationPropertyNamesQueryHandler : IRequestHandler<PersonnelSegmentSpecificationPropertyNamesQuery, IEnumerable<string>>
{
    private readonly IPersonnelSegmentSpecificationRepository _personnelSegmentSpecificationRepository;
    public PersonnelSegmentSpecificationPropertyNamesQueryHandler(IPersonnelSegmentSpecificationRepository personnelSegmentSpecificationRepository)
    {
        _personnelSegmentSpecificationRepository = personnelSegmentSpecificationRepository;
    }
    public async Task<IEnumerable<string>> Handle(PersonnelSegmentSpecificationPropertyNamesQuery request, CancellationToken cancellationToken)
    {
       var personnelSegmentSpecifications = await _personnelSegmentSpecificationRepository.GetPersonnelSegmentSpecifications();

        var propertyNames = personnelSegmentSpecifications
            .SelectMany(pss => pss.Properties)
            .Select(p => p.Name)
            .Distinct()
            .ToList();

        return propertyNames;
    }
}
