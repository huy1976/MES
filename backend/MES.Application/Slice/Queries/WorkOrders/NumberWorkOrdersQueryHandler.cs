namespace MesMicroservice.Application.Queries.WorkOrders
{
    public class NumberWorkOrdersQueryHandler : IRequestHandler<NumberWorkOrdersQuery, int>
    {
        private readonly IWorkOrderRepository _workOrderRepository;

        public NumberWorkOrdersQueryHandler(IWorkOrderRepository workOrderRepository)
        {
            _workOrderRepository = workOrderRepository;
        }

        public async Task<int> Handle(NumberWorkOrdersQuery request, CancellationToken cancellationToken)
        {
            var nextId = await _workOrderRepository.GetQuantityWorkOrders();
            while (await _workOrderRepository.ExistsAsync($"LSX{nextId}"))
            {
                nextId++;
            }

            return nextId;
        }
    }
}
