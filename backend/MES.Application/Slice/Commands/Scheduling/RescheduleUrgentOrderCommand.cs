using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesMicroservice.Application.Commands.Scheduling
{
    public class RescheduleUrgentOrderCommand : IRequest<Dictionary<string, object>>
    {
        public List<int> UrgentWorkOrderIds { get; set; } = new();
        public List<int> ReschedulableWorkOrderIds { get; set; } = new();
        public EAlgorithmType AlgorithmType { get; set; } = EAlgorithmType.TabuSearch;
    }
}
