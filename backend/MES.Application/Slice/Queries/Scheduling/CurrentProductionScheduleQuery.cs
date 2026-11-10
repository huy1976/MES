using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesMicroservice.Application.Queries.Scheduling
{
    public class CurrentProductionScheduleQuery : IRequest<List<WorkOrderViewDTO>>
    {
        public List<EStatus> Statuses { get; set; }

        public CurrentProductionScheduleQuery()
        {
            Statuses = new List<EStatus>
            {
                EStatus.ReadyForProduction,
                EStatus.InProduction,
                EStatus.Completed
            };
        }

        public CurrentProductionScheduleQuery(List<EStatus> statuses)
        {
            Statuses = statuses;
        }
    }
}
