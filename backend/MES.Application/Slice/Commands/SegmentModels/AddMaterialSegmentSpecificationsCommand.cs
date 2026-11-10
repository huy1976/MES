global using MesMicroservice.Application.DTOs.SegmentModelDTOs;

namespace MesMicroservice.Application.Commands.SegmentModels
{
    [DataContract]
    public class AddMaterialSegmentSpecificationsCommand : IRequest<bool>
    {
        [DataMember]
        public string ProcessSegmentId { get; set; }
        [DataMember]
        public List<MaterialSegmentSpecificationDTO> MaterialSegmentSpecifications { get; set; }

        public AddMaterialSegmentSpecificationsCommand(string processSegmentId, List<MaterialSegmentSpecificationDTO> materialSegmentSpecifications)
        {
            ProcessSegmentId = processSegmentId;
            MaterialSegmentSpecifications = materialSegmentSpecifications;
        }
    }


}