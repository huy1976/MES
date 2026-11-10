namespace MesMicroservice.Application.Commands.Persons
{
    public class CreatePersonnelCapabilityCommandHandler : IRequestHandler<CreatePersonnelCapabilityCommand, bool>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IPersonnelCapabilityRepository _personnelCapabilityRepository;

        public CreatePersonnelCapabilityCommandHandler(IPersonRepository personRepository, IPersonnelCapabilityRepository personnelCapabilityRepository)
        {
            _personRepository = personRepository;
            _personnelCapabilityRepository = personnelCapabilityRepository;
        }

        public async Task<bool> Handle(CreatePersonnelCapabilityCommand request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.GetAsync(request.PersonId);
            if (person is null)
            {
                throw new ResourceNotFoundException($"Person with ID {request.PersonId} not found.");
            }

            var properties = request.Properties.Select(x => new PersonnelCapabilityProperty(
                x.PropertyId,
                x.Description,
                new Value(x.ValueString, x.ValueType),
                x.ValueUnitOfMeasure,
                x.Name))
                .ToList();

            var capability = new PersonnelCapability(personnelCapabilityId: request.PersonnelCapabilityId,
                                                             description: request.Description ?? "--",
                                                             hierarchyScope: request.HierarchyScope ?? "--",
                                                             capabilityType: request.CapabilityType,
                                                             validFrom: request.ValidFrom ?? DateTime.UtcNow,
                                                             validTo: request.ValidTo ?? DateTime.MinValue,
                                                             quantity: request.Quantity,
                                                             valueUnitOfMeasure: request.ValueUnitOfMeasure,
                                                             properties: properties,
                                                             person: person);

            await _personnelCapabilityRepository.Add(capability);

            return await _personnelCapabilityRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
