namespace MesMicroservice.Application.Queries.MaterialDefinitions
{
    public class MaterialDefinitionIdsQueryHandler : IRequestHandler<MaterialDefinitionIdsQuery, IEnumerable<string>>
    {
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

        public MaterialDefinitionIdsQueryHandler(IMaterialDefinitionRepository materialDefinitionRepository)
        {
            _materialDefinitionRepository = materialDefinitionRepository;
        }

        public async Task<IEnumerable<string>> Handle(MaterialDefinitionIdsQuery request, CancellationToken cancellationToken)
        {
            var materialDefinitions = await _materialDefinitionRepository.GetMaterialDefinitions();
            // Lọc theeo ClassId
            materialDefinitions = materialDefinitions.Where(md => md.MaterialClass.ResourceId == "TP").ToList();

            var materialDefinitionIds = materialDefinitions.Select(md => md.ResourceId).ToList();

            return materialDefinitionIds;
        }
    }
}
