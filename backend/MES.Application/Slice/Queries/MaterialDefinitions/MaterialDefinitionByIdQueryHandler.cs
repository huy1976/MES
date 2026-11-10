global using MesMicroservice.Application.DTOs.MaterialDTOs;

namespace MesMicroservice.Application.Queries.MaterialDefinitions
{
    public class MaterialDefinitionByIdQueryHandler : IRequestHandler<MaterialDefinitionByIdQuery, MaterialDTO>
    {
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

        public MaterialDefinitionByIdQueryHandler(IMaterialDefinitionRepository materialDefinitionRepository)
        {
            _materialDefinitionRepository = materialDefinitionRepository;
        }

        public async Task<MaterialDTO> Handle(MaterialDefinitionByIdQuery request, CancellationToken cancellationToken)
        {
            var materialDefinition = await _materialDefinitionRepository.GetAsync(request.MaterialDefinitionId);
            if (materialDefinition is null)
            {
                throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialDefinitionId);
            }

            var materialDTO = new MaterialDTO(
                materialDefinition.ResourceId,
                materialDefinition.Name,
                materialDefinition.PrimaryUnit
            );

            return materialDTO;
        }
    }
}
