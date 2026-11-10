namespace MesMicroservice.Application.Queries.QualityStandards
{
    public class GetQualityStandardsByMaterialIdQueryHandler : IRequestHandler<GetQualityStandardsByMaterialIdQuery, List<string>>
    {
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

        public GetQualityStandardsByMaterialIdQueryHandler(IMaterialDefinitionRepository materialDefinitionRepository)
        {
            _materialDefinitionRepository = materialDefinitionRepository;
        }

        public async Task<List<string>> Handle(GetQualityStandardsByMaterialIdQuery request, CancellationToken cancellationToken)
        {
            var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialId);
            if (materialDefinition is null)
            {
                throw new ResourceNotFoundException($"MaterialDefinition with ID {request.MaterialId} not found.");
            }

            return materialDefinition.QualityStandards.Select(od => od.ResourceId).ToList();
        }
    }
}
