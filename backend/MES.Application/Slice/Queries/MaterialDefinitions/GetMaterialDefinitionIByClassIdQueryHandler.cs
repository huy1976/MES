namespace MesMicroservice.Application.Queries.MaterialDefinitions
{
    public class GetMaterialDefinitionIByClassIdQueryHandler : IRequestHandler<GetMaterialDefinitionIByClassIdQuery, IEnumerable<MateriaIdsDTO>>
    {
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

        public GetMaterialDefinitionIByClassIdQueryHandler(IMaterialDefinitionRepository materialDefinitionRepository)
        {
            _materialDefinitionRepository = materialDefinitionRepository;
        }

        public async Task<IEnumerable<MateriaIdsDTO>> Handle(GetMaterialDefinitionIByClassIdQuery request, CancellationToken cancellationToken)
        {
            var materialDefinitions = await _materialDefinitionRepository.GetByClassId(request.ClassId);

            var result = materialDefinitions.Select(md => new MateriaIdsDTO
            {
                MaterialId = md.ResourceId,
                MaterialName = md.Name
            });

            return result;
        }
    }
}
