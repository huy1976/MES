global using MesMicroservice.Application.Commands.Equipments;

namespace MesMicroservice.Application.Commands.WorkOrders;
public class CreateWorkOrderCommand : IRequest<bool>
{
    public string ManufacturingOrderId { get; set; }
    public string WorkOrderId { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public EWorkOrderStatus WorkOrderStatus { get; set; }
    public List<string> PrerequisiteOperations { get; set; }
    public string? WorkCenter { get; set; }
    public List<CreateEquipmentRequirementViewModel> EquipmentRequirements { get; set; }

    public CreateWorkOrderCommand(string manufacturingOrderId, string workOrderId, DateTime dueDate, DateTime startTime, DateTime endTime, EWorkOrderStatus workOrderStatus, List<string> prerequisiteOperations, string? workCenter, List<CreateEquipmentRequirementViewModel> equipmentRequirements)
    {
        ManufacturingOrderId = manufacturingOrderId;
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
