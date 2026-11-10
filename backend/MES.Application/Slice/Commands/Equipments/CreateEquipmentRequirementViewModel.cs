namespace MesMicroservice.Application.Commands.Equipments
{
    public class CreateEquipmentRequirementViewModel
    {
        public List<string> EquipmentClassIds { get; set; }
        public int Quantity { get; set; }

        public CreateEquipmentRequirementViewModel(List<string> equipmentClassIds, int quantity)
        {
            EquipmentClassIds = equipmentClassIds;
            Quantity = quantity;
        }
    }
}
