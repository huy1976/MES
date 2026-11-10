namespace MesMicroservice.Application.Commands.SegmentModels
{
    [DataContract]
    public class AddEquipmentSegmentSpecificationsCommand : IRequest<bool>
    {
        [DataMember]
        public string ProcessSegmentId { get; set; }
        [DataMember]
        public List<EquipmentSegmentSpecificationDTO> EquipmentSegmentSpecifications { get; set; }

        public AddEquipmentSegmentSpecificationsCommand(string processSegmentId, List<EquipmentSegmentSpecificationDTO> equipmentSegmentSpecifications)
        {
            ProcessSegmentId = processSegmentId;
            EquipmentSegmentSpecifications = equipmentSegmentSpecifications;
        }
    }
}
