namespace MesMicroservice.Application.Commands.PersonnelClasses
{
    [DataContract]
    public class CreatePersonnelClassCommand : IRequest<bool>
    {
        [DataMember]
        public string PersonnelClassId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public List<SavePropertyViewModel> Properties { get; set; }

        public CreatePersonnelClassCommand(string personnelClassId, string name, string description, List<SavePropertyViewModel> properties)
        {
            PersonnelClassId = personnelClassId;
            Name = name;
            Description = description;
            Properties = properties;
        }
    }
}
