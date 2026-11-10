using MesMicroservice.Domain.DomainEvents;

namespace MesMicroservice.Application.Services
{
    public class CompletionTimePredictor
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IMediator _mediator;

        public CompletionTimePredictor(IWorkOrderRepository workOrderRepository, IMediator mediator)
        {
            _workOrderRepository = workOrderRepository;
            _mediator = mediator;
        }

        public async Task UpdatePrediction()
        {
            var runningWorkOrders = await _workOrderRepository.GetListByStatus(EStatus.InProduction);

            foreach (var runningWorkOrder in runningWorkOrders)
            {
                var predictedCompletionTime = runningWorkOrder.PredictCompletionTime();

                var predictionChangedEvent = new WorkOrderCompletionTimePredictedEvent(runningWorkOrder, predictedCompletionTime);

                await _mediator.Publish(predictionChangedEvent);
            }
        }
    }

}
