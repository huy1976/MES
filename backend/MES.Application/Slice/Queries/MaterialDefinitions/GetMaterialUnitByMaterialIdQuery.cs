namespace MesMicroservice.Application.Queries.MaterialDefinitions
{
    public class GetMaterialUnitByMaterialIdQuery : IRequest<List<MaterialUnitDTO>>
    {
        public string MaterialDefinitionId { get; set; } = string.Empty;

    } 
}
