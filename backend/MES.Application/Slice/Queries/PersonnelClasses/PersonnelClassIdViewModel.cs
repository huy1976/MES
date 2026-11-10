namespace MesMicroservice.Application.Queries.PersonnelClasses;

public class PersonnelClassIdViewModel
{
    public string Id {  get; set; }
    public string Name { get; set; }

    public PersonnelClassIdViewModel(string id, string name)
    {
        Id = id;
        Name = name;
    }
}
