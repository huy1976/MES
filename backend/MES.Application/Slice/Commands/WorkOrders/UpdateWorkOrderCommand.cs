using MesMicroservice.Domain.AggregateModels.WorkOrderAggregate;

namespace MesMicroservice.Application.Commands.WorkOrders;

public class UpdateWorkOrderCommand : IRequest<bool>
{
    public string ManufacturingOrderId { get; set; }
    public string WorkOrderId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime? ActuallyStartTime { get; set; }
    public DateTime? ActuallyEndTime { get; set; }
    public EStatus WorkOrderStatus { get; set; }
    public string WorkCenter { get; set; }

    public UpdateWorkOrderCommand(string manufacturingOrderId, string workOrderId, DateTime startTime, DateTime endTime, DateTime? actuallyStartTime, DateTime? actuallyEndTime, EStatus workOrderStatus, string workCenter)
    {
        ManufacturingOrderId = manufacturingOrderId;
        WorkOrderId = workOrderId;
        StartTime = startTime;
        EndTime = endTime;
        ActuallyStartTime = actuallyStartTime;
        ActuallyEndTime = actuallyEndTime;
        WorkOrderStatus = workOrderStatus;
        WorkCenter = workCenter;
    }
}
