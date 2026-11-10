namespace MesMicroservice.Application.Queries.MaterialDefinitions
{
    public class MaterialDefinitionByIdQuery : IRequest<MaterialDTO>
    {
        public string MaterialDefinitionId { get; set; }

        public MaterialDefinitionByIdQuery(string materialDefinitionId)
        {
            MaterialDefinitionId = materialDefinitionId;
        }
    
    }
}
