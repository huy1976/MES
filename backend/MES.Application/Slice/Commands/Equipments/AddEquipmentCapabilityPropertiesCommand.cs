namespace MesMicroservice.Application.Commands.Equipments
{
    public class AddEquipmentCapabilityPropertiesCommand : IRequest<bool>
    {
        [DataMember]
        public string EquipmentCapabilityId { get; set; }
        [DataMember]
        public List<SavePropertyViewModel> Properties { get; set; }

        public AddEquipmentCapabilityPropertiesCommand(string equipmentCapabilityId, List<SavePropertyViewModel> properties)
        {
            EquipmentCapabilityId = equipmentCapabilityId;
            Properties = properties;
        }
    }
}
