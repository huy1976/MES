namespace MesMicroservice.Application.Queries.WorkOrders
{
    public class WorkOrderByStatusesQuery : IRequest<List<WorkOrderViewDTO>>
    {
        public List<EStatus> Statuses { get; set; }

        public WorkOrderByStatusesQuery(List<EStatus> statuses)
        {
            Statuses = statuses;
        }
    }
}
