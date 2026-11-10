namespace MesMicroservice.Application.Commands.QualityStandards
{
    public class AddMaterialIntoQualityStandardCommand : IRequest<bool>
    {
        public string QualityStandardId { get; set; }
        public List<string> MaterialDefinitionIds { get; set; }

        public AddMaterialIntoQualityStandardCommand(string qualityStandardId, List<string> materialDefinitionIds)
        {
            QualityStandardId = qualityStandardId;
            MaterialDefinitionIds = materialDefinitionIds;
        }
    }
}
