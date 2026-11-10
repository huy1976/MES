using System;
using System.Collections.Generic;
using System.Text;

namespace MES.Domain.DomainEvent
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
