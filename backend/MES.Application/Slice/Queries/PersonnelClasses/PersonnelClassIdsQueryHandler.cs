
namespace MesMicroservice.Application.Queries.PersonnelClasses;

public class PersonnelClassIdsQueryHandler : IRequestHandler<PersonnelClassIdsQuery, IEnumerable<PersonnelClassIdViewModel>>
{
    private readonly IPersonnelClassRepository _personnelClassRepository;
    public PersonnelClassIdsQueryHandler(IPersonnelClassRepository personnelClassRepository)
    {
        _personnelClassRepository = personnelClassRepository;
    }
    public async Task<IEnumerable<PersonnelClassIdViewModel>> Handle(PersonnelClassIdsQuery request, CancellationToken cancellationToken)
    {
        var personnelClasses = await _personnelClassRepository.GetPersonnelClasses();

        return personnelClasses.Select(x => new PersonnelClassIdViewModel(x.ResourceId, x.Name));
    }
}
