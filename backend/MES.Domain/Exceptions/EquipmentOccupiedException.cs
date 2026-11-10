namespace MesMicroservice.Domain.Exceptions;
[Serializable]
public class EquipmentOccupiedException : Exception
{
    public string EquipmentId { get; } = "";
    public string WorkOrderId { get; } = "";

    public EquipmentOccupiedException() : base()
    {
    }

    public EquipmentOccupiedException(string? message) : base(message)
    {
    }

    public EquipmentOccupiedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
    public EquipmentOccupiedException(string equipmentId, string workOrderId) : this($"Equipment with ID '{equipmentId}' is occupied by work order with ID '{workOrderId}'")
    {
        EquipmentId = equipmentId;
        WorkOrderId = workOrderId;
    }
}
