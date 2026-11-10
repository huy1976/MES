namespace MesMicroservice.Application.Commands.SegmentModels
{
    [DataContract]
    public class AddPersonnelSegmentSpecificationsCommand : IRequest<bool>
    {
        [DataMember]
        public string ProcessSegmentId { get; set; }
        [DataMember]
        public List<PersonnelSegmentSpecificationDTO> PersonnelSegmentSpecifications { get; set; }

        public AddPersonnelSegmentSpecificationsCommand(string processSegmentId, List<PersonnelSegmentSpecificationDTO> personnelSegmentSpecifications)
        {
            ProcessSegmentId = processSegmentId;
            PersonnelSegmentSpecifications = personnelSegmentSpecifications;
        }
    }

}
