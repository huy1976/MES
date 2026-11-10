namespace MesMicroservice.Application.Commands.Persons
{
    [DataContract]
    public class CreatePersonCommand : IRequest<bool>
    {
        [DataMember]
        public string PersonId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public List<SavePropertyViewModel> Properties { get; set; }
        [DataMember]
        public string PersonClass { get; set; }

        public CreatePersonCommand(string personId, string name, string description, List<SavePropertyViewModel> properties, string personClass)
        {
            PersonId = personId;
            Name = name;
            Description = description;
            Properties = properties;
            PersonClass = personClass;
        }
    }
}
