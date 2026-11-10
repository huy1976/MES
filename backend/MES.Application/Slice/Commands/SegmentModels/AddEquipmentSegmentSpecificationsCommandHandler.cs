namespace MesMicroservice.Application.Commands.SegmentModels
{
    public class AddEquipmentSegmentSpecificationsCommandHandler : IRequestHandler<AddEquipmentSegmentSpecificationsCommand, bool>
    {
        private readonly IEquipmentSegmentSpecificationRepository _equipmentSegmentSpecificationRepository;
        private readonly IEquipmentClassRepository _equipmentClassRepository;
        private readonly IProcessSegmentRepository _processSegmentRepository;

        public AddEquipmentSegmentSpecificationsCommandHandler(IEquipmentSegmentSpecificationRepository equipmentSegmentSpecificationRepository, IEquipmentClassRepository equipmentClassRepository, IProcessSegmentRepository processSegmentRepository)
        {
            _equipmentSegmentSpecificationRepository = equipmentSegmentSpecificationRepository;
            _equipmentClassRepository = equipmentClassRepository;
            _processSegmentRepository = processSegmentRepository;
        }

        public async Task<bool> Handle(AddEquipmentSegmentSpecificationsCommand request, CancellationToken cancellationToken)
        {
            var processSegment = await _processSegmentRepository.GetAsync(request.ProcessSegmentId);
            if (processSegment is null)
            {
                throw new ResourceNotFoundException($"ProcessSegment with ID {request.ProcessSegmentId} not found.");
            }

            foreach (var specDto in request.EquipmentSegmentSpecifications)
            {
                var equipmentClass = await _equipmentClassRepository.GetAsync(specDto.equipmentClassId);
                if (equipmentClass is null)
                {
                    throw new ResourceNotFoundException($"MaterialDefinition with ID {specDto.equipmentClassId} not found.");
                }

                var properties = new List<EquipmentSegmentSpecificationProperty>();
                if (specDto.Properties.Count() != 0)
                {
                    properties = specDto.Properties.ConvertAll(x => new EquipmentSegmentSpecificationProperty(
                        x.PropertyId,
                        x.Description,
                        new Value(x.ValueString, x.ValueType),
                        x.ValueUnitOfMeasure,
                        x.Name));
                }

                var materialSegmentSpecification = new EquipmentSegmentSpecification(equipmentSegmentSpecificationId: specDto.EquipmentSegmentSpecificationId,
                                                                                    description: specDto.Description ?? "--",
                                                                                    equipmentUse: specDto.EquipmentUse ?? EEquipmentUse.Production,
                                                                                    quantity: specDto.Quantity,
                                                                                    valueUnitOfMeasure: specDto.ValueUnitOfMeasure,
                                                                                    processSegment: processSegment,
                                                                                    equipmentClass: equipmentClass,
                                                                                    properties: properties);

                await _equipmentSegmentSpecificationRepository.Add(materialSegmentSpecification);
            }

            return await _equipmentSegmentSpecificationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
