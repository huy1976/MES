namespace MesMicroservice.Application.Queries.Persons;

public class PersonViewModel
{
    public string PersonId { get; set; }
    public string Name { get; set; }
    public EStatus PersonStatus { get; set; }
    public string? Description { get; set; }
    public PersonnelClassViewModel PersonnelClass { get; set; }
    public List<PropertyViewModel> Properties { get; set; }
    public List<PersonnelCapabilityViewModel> PersonnelCapabilities { get; set; }
    public List<PersonWorkOrder> PersonWorkOrders { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public PersonViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }
    public PersonViewModel(string personId, string name, EStatus personStatus, string? description, PersonnelClassViewModel personnelClass, List<PropertyViewModel> properties, List<PersonnelCapabilityViewModel> personnelCapabilities, List<PersonWorkOrder> personWorkOrders)
    {
        PersonId = personId;
        Name = name;
        PersonStatus = personStatus;
        Description = description;
        PersonnelClass = personnelClass;
        Properties = properties;
        PersonnelCapabilities = personnelCapabilities;
        PersonWorkOrders = personWorkOrders;
    }
}
