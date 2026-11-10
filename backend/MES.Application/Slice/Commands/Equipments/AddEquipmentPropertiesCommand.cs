namespace MesMicroservice.Application.Commands.Equipments
{
    [DataContract]
    public class AddEquipmentPropertiesCommand : IRequest<bool>
    {
        [DataMember]
        public string EquipmentId { get; set; }
        [DataMember]
        public List<SavePropertyViewModel> Properties { get; set; }

        public AddEquipmentPropertiesCommand(string equipmentId, List<SavePropertyViewModel> properties)
        {
            EquipmentId = equipmentId;
            Properties = properties;
        }
    }
}
