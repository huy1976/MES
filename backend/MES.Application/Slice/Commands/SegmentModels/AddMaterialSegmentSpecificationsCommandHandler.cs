namespace MesMicroservice.Application.Commands.SegmentModels
{
    public class AddMaterialSegmentSpecificationsCommandHandler : IRequestHandler<AddMaterialSegmentSpecificationsCommand, bool>
    {
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
        private readonly IProcessSegmentRepository _processSegmentRepository;
        private readonly IMaterialSegmentSpecificationRepository _materialSegmentSpecificationRepository;

        public AddMaterialSegmentSpecificationsCommandHandler(IMaterialDefinitionRepository materialDefinitionRepository, IProcessSegmentRepository processSegmentRepository, IMaterialSegmentSpecificationRepository materialSegmentSpecificationRepository)
        {
            _materialDefinitionRepository = materialDefinitionRepository;
            _processSegmentRepository = processSegmentRepository;
            _materialSegmentSpecificationRepository = materialSegmentSpecificationRepository;
        }

        public async Task<bool> Handle(AddMaterialSegmentSpecificationsCommand request, CancellationToken cancellationToken)
        {
            var processSegment = await _processSegmentRepository.GetAsync(request.ProcessSegmentId);
            if (processSegment is null)
            {
                throw new ResourceNotFoundException($"ProcessSegment with ID {request.ProcessSegmentId} not found.");
            }

            foreach (var specDto in request.MaterialSegmentSpecifications)
            {
                var materialDefinition = await _materialDefinitionRepository.GetAsync(specDto.MaterialDefinitionId);
                if (materialDefinition is null)
                {
                    throw new ResourceNotFoundException($"MaterialDefinition with ID {specDto.MaterialDefinitionId} not found.");
                }

                var properties = new List<MaterialSegmentSpecificationProperty>();
                if (specDto.Properties.Count() != 0)
                {
                    properties = specDto.Properties.ConvertAll(x => new MaterialSegmentSpecificationProperty(
                        x.PropertyId,
                        x.Description,
                        new Value(x.ValueString, x.ValueType),
                        x.ValueUnitOfMeasure,
                        x.Name));
                }

                var materialSegmentSpecification = new MaterialSegmentSpecification(materialSegmentSpecificationId: specDto.MaterialSegmentSpecificationId,
                                                                                    description: specDto.Description ?? "--",
                                                                                    assemblyType: specDto.AssemblyType ?? EAssemblyType.None,
                                                                                    assemblyRelationship: specDto.AssemblyRelationship ?? EAssemblyRelationship.None,
                                                                                    materialUse: specDto.MaterialUse ?? EMaterialUse.Consumable,
                                                                                    quantity: new Value("1",EValueType.Decimal),
                                                                                    valueUnitOfMeasure: "Loai",
                                                                                    processSegment: processSegment,
                                                                                    materialDefinition: materialDefinition,
                                                                                    properties: properties);

                await _materialSegmentSpecificationRepository.Add(materialSegmentSpecification);
            }

            return await _materialSegmentSpecificationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
