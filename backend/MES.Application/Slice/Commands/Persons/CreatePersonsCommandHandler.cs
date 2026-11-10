namespace MesMicroservice.Application.Commands.Persons
{
    public class CreatePersonsCommandHandler : IRequestHandler<CreatePersonsCommand, bool>
    {

        private readonly IPersonRepository _personRepository;
        private readonly IPersonnelClassRepository _personnelClassRepository;

        public CreatePersonsCommandHandler(IPersonRepository personRepository, IPersonnelClassRepository personClassRepository)
        {
            _personRepository = personRepository;
            _personnelClassRepository = personClassRepository;
        }

        public async Task<bool> Handle(CreatePersonsCommand request, CancellationToken cancellationToken)
        {
            foreach (var createPersonCommand in request.createPersonCommands)
            {
                var properties = createPersonCommand.Properties.ConvertAll(x => new PersonProperty(
                    x.PropertyId,
                    x.Description,
                    new Value(x.ValueString, x.ValueType),
                    x.ValueUnitOfMeasure,
                    x.Name));

                var personnelClass = await _personnelClassRepository
                    .GetAsync(createPersonCommand.PersonClass) ?? throw new ResourceNotFoundException(nameof(PersonnelClass), createPersonCommand.PersonClass);

                var person = new Person(personId: createPersonCommand.PersonId,
                                        name: createPersonCommand.Name,
                                        description: createPersonCommand.Description,
                                        personnelClass: personnelClass,
                                        properties: properties);

                await _personRepository.Add(person);
            }

            return await _personRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }

    }
}
