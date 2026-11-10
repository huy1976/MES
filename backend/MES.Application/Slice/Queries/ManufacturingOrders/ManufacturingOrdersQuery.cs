global using MesMicroservice.Application.DTOs.ManufacturingOrderDTOs;

namespace MesMicroservice.Application.Queries.ManufacturingOrders;

public class ManufacturingOrdersQuery : PaginatedQuery, IRequest<IEnumrable<ManufacturingOrderDTO>>
{
    public string? IdStartedWith { get; set; }
}
