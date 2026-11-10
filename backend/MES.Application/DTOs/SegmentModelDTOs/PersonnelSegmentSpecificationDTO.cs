namespace MesMicroservice.Application.DTOs.SegmentModelDTOs
{
    public class PersonnelSegmentSpecificationDTO
    {
        public string PersonnelSegmentSpecificationId { get; set; }
        public string? Description { get; set; }
        public EPersonnelUse?  PersonnelUse { get; set; }
        public string personClassId { get; set; }
        public List<SavePropertyViewModel> Properties { get; set; }
        public Value Quantity { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }

        public PersonnelSegmentSpecificationDTO(string personnelSegmentSpecificationId, string? description, EPersonnelUse? personnelUse, string personClassId, List<SavePropertyViewModel> properties, Value quantity, string valueUnitOfMeasure)
        {
            PersonnelSegmentSpecificationId = personnelSegmentSpecificationId;
            Description = description;
            PersonnelUse = personnelUse;
            this.personClassId = personClassId;
            Properties = properties;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
        }
    }
}

