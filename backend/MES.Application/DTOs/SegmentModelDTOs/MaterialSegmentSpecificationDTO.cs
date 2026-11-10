global using MesMicroservice.Application.Commands;

namespace MesMicroservice.Application.DTOs.SegmentModelDTOs
{
    public class MaterialSegmentSpecificationDTO
    {
        public string MaterialSegmentSpecificationId { get; set; }
        public string? Description { get; set; }
        public EAssemblyType? AssemblyType { get; set; }
        public EAssemblyRelationship? AssemblyRelationship { get; set; }
        public EMaterialUse? MaterialUse { get; set; }
        public string MaterialDefinitionId { get; set; }
        public List<SavePropertyViewModel> Properties { get; set; }

        public MaterialSegmentSpecificationDTO(string materialSegmentSpecificationId, string? description, EAssemblyType? assemblyType, EAssemblyRelationship? assemblyRelationship, EMaterialUse? materialUse, string materialDefinitionId, List<SavePropertyViewModel> properties)
        {
            MaterialSegmentSpecificationId = materialSegmentSpecificationId;
            Description = description;
            AssemblyType = assemblyType;
            AssemblyRelationship = assemblyRelationship;
            MaterialUse = materialUse;
            MaterialDefinitionId = materialDefinitionId;
            Properties = properties;
        }
    }


}
