namespace MesMicroservice.Application.Queries.MaterialDefinitions
{
    public class MaterialDefinitionByMaterialIdQuery : IRequest<MaterialDTO>
    {
        public string MaterialId { get; set; }

        public MaterialDefinitionByMaterialIdQuery(string materialId)
        {
            MaterialId = materialId;
        }
    }
}
