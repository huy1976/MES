namespace MesMicroservice.Application.Queries.PersonnelClasses;

public class PersonnelClassViewModel
{
    public string PersonnelClassId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<string> Persons { get; set; }
    public List<PropertyViewModel> Properties { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public PersonnelClassViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public PersonnelClassViewModel(string name)
    {
        Name = name;
    }

    public PersonnelClassViewModel(string personnelClassId, string name, string? description)
    {
        PersonnelClassId = personnelClassId;
        Name = name;
        Description = description;
    }

    public PersonnelClassViewModel(string personnelClassId, string name, string? description, List<string> persons, List<PropertyViewModel> properties)
    {
        PersonnelClassId = personnelClassId;
        Name = name;
        Description = description;
        Persons = persons;
        Properties = properties;
    }
}
