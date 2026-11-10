namespace MesMicroservice.Application.Commands.SegmentModels
{
    public class AddPersonnelSegmentSpecificationsCommandHandler : IRequestHandler<AddPersonnelSegmentSpecificationsCommand, bool>
    {
        private readonly IPersonnelSegmentSpecificationRepository _personnelSegmentSpecificationRepository;
        private readonly IPersonnelClassRepository _personnelClassRepository;
        private readonly IProcessSegmentRepository _processSegmentRepository;

        public AddPersonnelSegmentSpecificationsCommandHandler(IPersonnelSegmentSpecificationRepository personnelSegmentSpecificationRepository, IPersonnelClassRepository personnelClassRepository, IProcessSegmentRepository processSegmentRepository)
        {
            _personnelSegmentSpecificationRepository = personnelSegmentSpecificationRepository;
            _personnelClassRepository = personnelClassRepository;
            _processSegmentRepository = processSegmentRepository;
        }

        public async Task<bool> Handle(AddPersonnelSegmentSpecificationsCommand request, CancellationToken cancellationToken)
        {
            var processSegment = await _processSegmentRepository.GetAsync(request.ProcessSegmentId);
            if (processSegment is null)
            {
                throw new ResourceNotFoundException($"ProcessSegment with ID {request.ProcessSegmentId} not found.");
            }

            foreach (var specDto in request.PersonnelSegmentSpecifications)
            {
                var personnelClass = await _personnelClassRepository.GetAsync(specDto.personClassId);
                if (personnelClass is null)
                {
                    throw new ResourceNotFoundException($"MaterialDefinition with ID {specDto.personClassId} not found.");
                }

                var properties = new List<PersonnelSegmentSpecificationProperty>();
                if (specDto.Properties.Count() != 0)
                {
                    properties = specDto.Properties.ConvertAll(x => new PersonnelSegmentSpecificationProperty(
                        x.PropertyId,
                        x.Description,
                        new Value(x.ValueString, x.ValueType),
                        x.ValueUnitOfMeasure,
                        x.Name));
                }

                var materialSegmentSpecification = new PersonnelSegmentSpecification(personnelSegmentSpecificationId: specDto.PersonnelSegmentSpecificationId,
                                                                                    description: specDto.Description ?? "--",
                                                                                    personnelUse: specDto.PersonnelUse ?? EPersonnelUse.Operator,
                                                                                    quantity: specDto.Quantity,
                                                                                    valueUnitOfMeasure: specDto.ValueUnitOfMeasure,
                                                                                    processSegment: processSegment,
                                                                                    personnelClass: personnelClass,
                                                                                    properties: properties);

                await _personnelSegmentSpecificationRepository.Add(materialSegmentSpecification);
            }

            return await _personnelSegmentSpecificationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }

    }
}
