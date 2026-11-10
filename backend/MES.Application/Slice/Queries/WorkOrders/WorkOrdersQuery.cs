namespace MesMicroservice.Application.Queries.WorkOrders;

public class WorkOrdersQuery : PaginatedQuery, IRequest<List<WorkOrderViewDTO>>
{
    public string? ManufacturingOrderId { get; set; }
    public string? IdStartedWith { get; set; }
    public EStatus? Status { get; set; }
    public string? WorkCenterId { get; set; }

}
