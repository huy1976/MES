using MesMicroservice.Application.Commands.Equipments;

namespace MesMicroservice.Application.Commands.WorkOrders;

[DataContract]
public class CreateWorkOrderViewModel
{
    [DataMember]
    public string WorkOrderId { get; set; }
    [DataMember]
    public DateTime DueDate { get; set; }
    [DataMember]
    public DateTime StartTime { get; set; }
    [DataMember]
    public DateTime EndTime { get; set; }
    [DataMember]
    public EWorkOrderStatus WorkOrderStatus { get; set; }
    [DataMember]
    public List<string> PrerequisiteOperations { get; set; }
    [DataMember]
    public string WorkCenter { get; set; }
    [DataMember]
    public List<CreateEquipmentRequirementViewModel> EquipmentRequirements { get; set; }

    public CreateWorkOrderViewModel(string workOrderId, DateTime dueDate, DateTime startTime, DateTime endTime, EWorkOrderStatus workOrderStatus, List<string> prerequisiteOperations, string workCenter, List<CreateEquipmentRequirementViewModel> equipmentRequirements)
    {
        WorkOrderId = workOrderId;
        DueDate = dueDate;
        StartTime = startTime;
        EndTime = endTime;
        WorkOrderStatus = workOrderStatus;
        PrerequisiteOperations = prerequisiteOperations;
        WorkCenter = workCenter;
        EquipmentRequirements = equipmentRequirements;
    }
}
