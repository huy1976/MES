using Microsoft.AspNetCore.SignalR;

namespace MesMicroservice.Application.Services
{
    public class CompletionTimePredictionsBuffer
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly Dictionary<ManufacturingOrder, DateTime> ManufacturingOrderCompletionTimes = new();
        private readonly Dictionary<WorkOrder, DateTime> WorkOrderCompletionTimes = new();

        public CompletionTimePredictionsBuffer(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task UpdateUsingWorkOrderCompletionTime(WorkOrder workOrder, DateTime workOrderCompletionTime)
        {
            if (!WorkOrderCompletionTimes.ContainsKey(workOrder))
            {
                WorkOrderCompletionTimes.Add(workOrder, workOrderCompletionTime);
            }

            var currentWorkOrderCompletionTime = WorkOrderCompletionTimes[workOrder];
            if (workOrderCompletionTime >= currentWorkOrderCompletionTime)
            {
                WorkOrderCompletionTimes[workOrder] = workOrderCompletionTime;
                await _hubContext.Clients.All.SendAsync("WorkOrderCompletionTimeChanged", workOrder.ManufacturingOrder.ManufacturingOrderId, workOrder.WorkOrderId, workOrderCompletionTime);
            }

            var manufacturingOrder = workOrder.ManufacturingOrder;
            if (!ManufacturingOrderCompletionTimes.ContainsKey(manufacturingOrder))
            {
                var completionTime = manufacturingOrder.WorkOrders.Max(x => x.EndTime);
                ManufacturingOrderCompletionTimes.Add(manufacturingOrder, completionTime!.Value);
            }

            var currentManufacturingOrderCompletionTime = ManufacturingOrderCompletionTimes[manufacturingOrder];
            if (workOrderCompletionTime >= currentManufacturingOrderCompletionTime)
            {
                ManufacturingOrderCompletionTimes[manufacturingOrder] = workOrderCompletionTime;
                await _hubContext.Clients.All.SendAsync("ManufacturingOrderCompletionTimeChanged", manufacturingOrder.ManufacturingOrderId, workOrderCompletionTime);
            }
        }

        private async Task NotifyCompletionTimePredictionChanged()
        {
            foreach (var workOrder in WorkOrderCompletionTimes.Keys)
            {
                var newCompletionTime = WorkOrderCompletionTimes[workOrder];
                await _hubContext.Clients.All.SendAsync("WorkOrderCompletionTimeChanged", workOrder.WorkOrderId, newCompletionTime);
            }

            foreach (var manufacturingOrder in ManufacturingOrderCompletionTimes.Keys)
            {
                var newCompletionTime = ManufacturingOrderCompletionTimes[manufacturingOrder];
                await _hubContext.Clients.All.SendAsync("ManufacturingOrderCompletionTimeChanged", manufacturingOrder.ManufacturingOrderId, newCompletionTime);
            }
        }

        ~CompletionTimePredictionsBuffer()
        {
            Task.Run(NotifyCompletionTimePredictionChanged);
        }
    }

}
