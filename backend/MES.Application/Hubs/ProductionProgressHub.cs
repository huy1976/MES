using Microsoft.AspNetCore.SignalR;

namespace MesMicroservice.Api.Hubs;

public sealed class ProductionProgressHub : Hub
{
    public Task JoinWorkOrderGroup(string workOrderId)
        => Groups.AddToGroupAsync(Context.ConnectionId, $"work-order:{workOrderId}");

    public Task LeaveWorkOrderGroup(string workOrderId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, $"work-order:{workOrderId}");

    // Worker gọi method này qua SignalR Client.
    public async Task ReceiveProgressFromWorker(WorkOrderProgressRealtimeDto dto)
    {
        await Clients.Group($"work-order:{dto.WorkOrderId}").SendAsync("workOrderProgressUpdated", dto);
        await Clients.All.SendAsync("workOrderProgressListUpdated", dto);
    }
}

public sealed class WorkOrderProgressRealtimeDto
{
    public required string WorkOrderId { get; init; }
    public required string Equipment { get; init; }
    public double TotalQuantity { get; init; }
    public double ProducedQuantity { get; init; }
    public double ErrorQuantity { get; init; }
    public double DeltaQuantity { get; init; }
    public double DeltaErrorQuantity { get; init; }
    public double ProgressPercentage { get; init; }
    public string UnitOfMeasure { get; init; } = "Kg";
    public DateTime Timestamp { get; init; }
    public string Status { get; init; } = "InProduction";
}
