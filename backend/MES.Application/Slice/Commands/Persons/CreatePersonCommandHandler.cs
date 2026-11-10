global using MesMicroservice.Domain.AggregateModels.PersonnelClassAggregate;

namespace MesMicroservice.Application.Commands.Persons
{
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, bool>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IPersonnelClassRepository _personnelClassRepository;

        public CreatePersonCommandHandler(IPersonRepository personRepository, IPersonnelClassRepository personClassRepository)
        {
            _personRepository = personRepository;
            _personnelClassRepository = personClassRepository;
        }


        public async Task<bool> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var properties = request.Properties.ConvertAll(x => new PersonProperty(
                x.PropertyId,
                x.Description,
                new Value(x.ValueString, x.ValueType),
                x.ValueUnitOfMeasure,
                x.Name));

            var personnelClass = await _personnelClassRepository
                .GetAsync(request.PersonClass) ?? throw new ResourceNotFoundException(nameof(PersonnelClass), request.PersonClass);

            var person = new Person(personId: request.PersonId, 
                                    name: request.Name,
                                    description: request.Description,
                                    personnelClass: personnelClass,
                                    properties: properties);

            await _personRepository.Add(person);

            return await _personRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
