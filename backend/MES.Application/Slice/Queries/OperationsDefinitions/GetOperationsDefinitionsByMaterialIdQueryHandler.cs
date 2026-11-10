namespace MesMicroservice.Application.Queries.OperationsDefinitions
{
    public class GetOperationsDefinitionsByMaterialIdQueryHandler : IRequestHandler<GetOperationsDefinitionsByMaterialIdQuery, List<string>>
    {
        private readonly IOperationsDefinitionRepository _operationsDefinitionRepository;
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

        public GetOperationsDefinitionsByMaterialIdQueryHandler(IOperationsDefinitionRepository operationsDefinitionRepository, IMaterialDefinitionRepository materialDefinitionRepository)
        {
            _operationsDefinitionRepository = operationsDefinitionRepository;
            _materialDefinitionRepository = materialDefinitionRepository;
        }

        public async Task<List<string>> Handle(GetOperationsDefinitionsByMaterialIdQuery request, CancellationToken cancellationToken)
        {
            var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialId);
            if (materialDefinition is null)
            {
                throw new ResourceNotFoundException($"MaterialDefinition with ID {request.MaterialId} not found.");
            }

            return materialDefinition.OperationsDefinitions.Select(od => od.ResourceId).ToList();
        }
    }
}
