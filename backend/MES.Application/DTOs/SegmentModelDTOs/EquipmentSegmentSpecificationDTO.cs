namespace MesMicroservice.Application.DTOs.SegmentModelDTOs
{
    public class EquipmentSegmentSpecificationDTO
    {
        public string EquipmentSegmentSpecificationId { get; set; }
        public string? Description { get; set; }
        public EEquipmentUse? EquipmentUse { get; set; }
        public string equipmentClassId { get; set; }
        public List<SavePropertyViewModel> Properties { get; set; }
        public Value Quantity { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }

        public EquipmentSegmentSpecificationDTO(string equipmentSegmentSpecificationId, string? description, EEquipmentUse? equipmentUse, string equipmentClassId, List<SavePropertyViewModel> properties, Value quantity, string valueUnitOfMeasure)
        {
            EquipmentSegmentSpecificationId = equipmentSegmentSpecificationId;
            Description = description;
            EquipmentUse = equipmentUse;
            this.equipmentClassId = equipmentClassId;
            Properties = properties;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
        }
    }
}
