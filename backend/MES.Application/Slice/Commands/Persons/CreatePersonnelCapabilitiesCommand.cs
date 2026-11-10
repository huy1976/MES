namespace MesMicroservice.Application.Commands.Persons
{
    public class CreatePersonnelCapabilitiesCommand : IRequest<bool>
    {
        public List<CreatePersonnelCapabilityCommand> createPersonnelCapabilityCommands { get; set; } = new List<CreatePersonnelCapabilityCommand>();
    }
}
