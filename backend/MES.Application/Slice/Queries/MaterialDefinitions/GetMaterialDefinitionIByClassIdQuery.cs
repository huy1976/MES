namespace MesMicroservice.Application.Queries.MaterialDefinitions
{
    public class GetMaterialDefinitionIByClassIdQuery : IRequest<IEnumerable<MateriaIdsDTO>>
    {
        public string ClassId { get; set; } = string.Empty;

        public GetMaterialDefinitionIByClassIdQuery(string classId)
        {
            ClassId = classId;
        }
    }
}
