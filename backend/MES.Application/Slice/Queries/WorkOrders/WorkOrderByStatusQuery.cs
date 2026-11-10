namespace MesMicroservice.Application.Queries.WorkOrders
{
    public class WorkOrderByStatusQuery : IRequest<List<WorkOrderViewDTO>>
    {
        public EStatus Status { get; set; }

        public WorkOrderByStatusQuery(EStatus status)
        {
            Status = status;
        }
    }
}
