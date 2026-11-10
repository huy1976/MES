namespace MesMicroservice.Application.Commands.Persons
{
    public class CreatePersonnelCapabilitiesCommandHandler : IRequestHandler<CreatePersonnelCapabilitiesCommand, bool>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IPersonnelCapabilityRepository _personnelCapabilityRepository;
        public CreatePersonnelCapabilitiesCommandHandler(IPersonRepository personRepository, IPersonnelCapabilityRepository personnelCapabilityRepository)
        {
            _personRepository = personRepository;
            _personnelCapabilityRepository = personnelCapabilityRepository;
        }

        public async Task<bool> Handle(CreatePersonnelCapabilitiesCommand request, CancellationToken cancellationToken)
        {
            foreach (var cmd in request.createPersonnelCapabilityCommands)
            {
                var person = await _personRepository.GetAsync(cmd.PersonId);
                if (person is null)
                {
                    throw new ResourceNotFoundException($"Person with ID {cmd.PersonId} not found.");
                }

                var properties = cmd.Properties.Select(x => new PersonnelCapabilityProperty(
                    x.PropertyId,
                    x.Description,
                    new Value(x.ValueString, x.ValueType),
                    x.ValueUnitOfMeasure,
                    x.Name))
                    .ToList();

                var capability = new PersonnelCapability(personnelCapabilityId: cmd.PersonnelCapabilityId,
                                                                 description: cmd.Description ?? "--",
                                                                 hierarchyScope: cmd.HierarchyScope ?? "--",
                                                                 capabilityType: cmd.CapabilityType,
                                                                 validFrom: cmd.ValidFrom ?? DateTime.UtcNow,
                                                                 validTo: cmd.ValidTo ?? DateTime.MinValue,
                                                                 quantity: cmd.Quantity,
                                                                 valueUnitOfMeasure: cmd.ValueUnitOfMeasure,
                                                                 properties: properties,
                                                                 person: person);

                await _personnelCapabilityRepository.Add(capability);
            }

            return await _personnelCapabilityRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
