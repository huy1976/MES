namespace MesMicroservice.Application.Commands.Scheduling
{
    public class ResultSchedulingForWorkOrdersCommand : IRequest<Dictionary<string, object>>
    {
        public List<int> WorkOrderIds { get; set; }
        public List<int> UrgentWorkOrderIds { get; set; } = new();
        public EAlgorithmType AlgorithmType { get; set; }

        public ResultSchedulingForWorkOrdersCommand(
            List<int> workOrderIds,
            EAlgorithmType algorithmType,
            List<int>? urgentWorkOrderIds = null)
        {
            WorkOrderIds = workOrderIds;
            AlgorithmType = algorithmType;
            UrgentWorkOrderIds = urgentWorkOrderIds?.Distinct().ToList() ?? new List<int>();
        }
    }
}