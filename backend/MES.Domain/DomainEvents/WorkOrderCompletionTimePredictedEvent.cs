using MesMicroservice.Domain.AggregateModels.WorkOrderAggregate;

namespace MesMicroservice.Domain.DomainEvents
{
    public class WorkOrderCompletionTimePredictedEvent : INotification
    {
        public WorkOrder WorkOrder { get; set; }
        public DateTime PredictedCompletionTime { get; set; }

        public WorkOrderCompletionTimePredictedEvent(WorkOrder workOrder, DateTime predictedCompletionTime)
        {
            WorkOrder = workOrder;
            PredictedCompletionTime = predictedCompletionTime;
        }
    }
}
