namespace MesMicroservice.Application.Queries.OperationsDefinitions
{
    public class GetOperationsDefinitionsByMaterialIdQuery : IRequest<List<string>>
    {
        public string MaterialId { get; set; }

        public GetOperationsDefinitionsByMaterialIdQuery(string materialId)
        {
            MaterialId = materialId;
        }
    }
}
