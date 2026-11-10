using MesMicroservice.Application.Commands.EquipmentClasses;

namespace MesMicroservice.Application.Commands.PersonnelClasses
{
    public class CreatePersonnelClassCommandHandler : IRequestHandler<CreatePersonnelClassCommand, bool>
    {
        private readonly IPersonnelClassRepository _personnelClassRepository;

        public CreatePersonnelClassCommandHandler(IPersonnelClassRepository personnelClassRepository)
        {
            _personnelClassRepository = personnelClassRepository;
        }

        public async Task<bool> Handle(CreatePersonnelClassCommand request, CancellationToken cancellationToken)
        {

            var properties = request.Properties.ConvertAll(x => new PersonnelClassProperty(
                x.PropertyId,
                x.Description,
                new Value(x.ValueString, x.ValueType),
                x.ValueUnitOfMeasure,
                x.Name));

            if (properties.Count == 0)
            {
                properties = new List<PersonnelClassProperty>();
            }


            var personnelClass = new PersonnelClass(classId: request.PersonnelClassId, 
                                                    name: request.Name,
                                                    description: request.Description,
                                                    properties: properties);

            await _personnelClassRepository.AddAsync(personnelClass);

            return await _personnelClassRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }

    }
}
