namespace MesMicroservice.Application.Queries.ManufacturingOrders
{
    public class ManufacturingOrdersByStatusQuery : IRequest<IEnumerable<ManufacturingOrderDTO>>
    {
        public EStatus Status { get; set; }

        public ManufacturingOrdersByStatusQuery(EStatus status)
        {
            Status = status;
        }
    }
}
