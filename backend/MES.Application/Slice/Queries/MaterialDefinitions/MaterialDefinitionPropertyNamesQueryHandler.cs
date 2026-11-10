namespace MesMicroservice.Application.Queries.MaterialDefinitions
{
    public class MaterialDefinitionPropertyNamesQueryHandler : IRequestHandler<MaterialDefinitionPropertyNamesQuery, IEnumerable<string>>
    {
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
        public MaterialDefinitionPropertyNamesQueryHandler(IMaterialDefinitionRepository materialDefinitionRepository)
        {
            _materialDefinitionRepository = materialDefinitionRepository;
        }
        public async Task<IEnumerable<string>> Handle(MaterialDefinitionPropertyNamesQuery request, CancellationToken cancellationToken)
        {
            var materialDefinitions = await _materialDefinitionRepository.GetMaterialDefinitions();

            var propertyNames = materialDefinitions
                .SelectMany(md => md.Properties)
                .Select(p => p.Name)
                .Distinct()
                .ToList();

            return propertyNames;
        }
    }
}
