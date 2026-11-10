namespace MesMicroservice.Application.Queries.QualityStandards
{
    public class GetQualityStandardsByMaterialIdQuery : IRequest<List<string>>
    {
        public string MaterialId { get; set; }

        public GetQualityStandardsByMaterialIdQuery(string materialId)
        {
            MaterialId = materialId;
        }
    }
}
