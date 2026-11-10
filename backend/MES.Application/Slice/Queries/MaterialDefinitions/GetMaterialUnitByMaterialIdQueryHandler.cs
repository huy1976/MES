namespace MesMicroservice.Application.Queries.MaterialDefinitions
{
    public class GetMaterialUnitByMaterialIdQueryHandler : IRequestHandler<GetMaterialUnitByMaterialIdQuery, List<MaterialUnitDTO>>
    {
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
        private readonly IMapper _mapper;

        public GetMaterialUnitByMaterialIdQueryHandler(IMaterialDefinitionRepository materialDefinitionRepository, IMapper mapper)
        {
            _materialDefinitionRepository = materialDefinitionRepository;
            _mapper = mapper;
        }

        public async Task<List<MaterialUnitDTO>> Handle(GetMaterialUnitByMaterialIdQuery request, CancellationToken cancellationToken)
        {
            var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId);

            if (materialDefinition is null)
            {
                throw new Exception($"Material Definition ID {request.MaterialDefinitionId} not found.");
            }

            var materialUnits = new List<MaterialUnitDTO>();
            if (materialDefinition.SecondaryUnits.Count() > 0)
            {
                foreach (var secondaryUnit in materialDefinition.SecondaryUnits)
                {
                    var unitDTO = new MaterialUnitDTO
                    {
                        MaterialDefinitionId = materialDefinition.ResourceId,
                        UnitId = secondaryUnit.UnitId,
                        UnitName = secondaryUnit.UnitName,
                        ConversionValueToPrimaryUnit = secondaryUnit.ConversionValueToPrimaryUnit
                    };
                    materialUnits.Add(unitDTO);
                }
            }

            return materialUnits;
        }
    }
}
