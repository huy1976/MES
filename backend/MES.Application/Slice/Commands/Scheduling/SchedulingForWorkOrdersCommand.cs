namespace MesMicroservice.Application.Commands.Scheduling
{
    public class SchedulingForWorkOrdersCommand : IRequest<Solution?>
    {
        public List<int> WorkOrderIds { get; set; }
        public List<int> UrgentWorkOrderIds { get; set; } = new();
        public EAlgorithmType AlgorithmType { get; set; }
        public bool UseAI { get; set; } = true;

        public SchedulingForWorkOrdersCommand(
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
