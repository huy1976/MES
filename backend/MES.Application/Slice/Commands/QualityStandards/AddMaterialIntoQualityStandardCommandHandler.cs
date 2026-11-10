namespace MesMicroservice.Application.Commands.QualityStandards
{
    public class AddMaterialIntoQualityStandardCommandHandler : IRequestHandler<AddMaterialIntoQualityStandardCommand, bool>
    {
        private readonly IQualityStandardRepository _qualityStandardRepository;
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

        public AddMaterialIntoQualityStandardCommandHandler(IQualityStandardRepository qualityStandardRepository, IMaterialDefinitionRepository materialDefinitionRepository)
        {
            _qualityStandardRepository = qualityStandardRepository;
            _materialDefinitionRepository = materialDefinitionRepository;
        }

        public async Task<bool> Handle(AddMaterialIntoQualityStandardCommand request, CancellationToken cancellationToken)
        {
            var qualityStandard = await _qualityStandardRepository.GetAsync(request.QualityStandardId);
            if (qualityStandard is null)
            {
                throw new Exception($"Quality Standard with ID {request.QualityStandardId} not found.");
            }

            var productMaterials= await _materialDefinitionRepository.GetByClassId("TP");

            foreach (var materialDefinitionId in request.MaterialDefinitionIds)
            {
                var materialDefinition = productMaterials.FirstOrDefault(md => md.ResourceId == materialDefinitionId);
                if (materialDefinition is null)
                {
                    throw new Exception($"Material Definition with ID {materialDefinitionId} not found or is not a product material.");
                }

                if (!qualityStandard.MaterialDefinitions.Contains(materialDefinition))
                {
                    qualityStandard.MaterialDefinitions.Add(materialDefinition);
                }
            }

            return await _qualityStandardRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
