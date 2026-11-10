
using MesMicroservice.Domain.AggregateModels.WorkOrderAggregate;

namespace MesMicroservice.Infrastructure.Repositories.OrderModelRepositories;

public class ManufacturingOrderRepository : BaseRepository, IManufacturingOrderRepository
{
    public ManufacturingOrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ManufacturingOrder> Add(ManufacturingOrder manufacturingOrder)
    {
        if (manufacturingOrder.IsTransient())
        {
            if (await ExistsAsync(manufacturingOrder.ManufacturingOrderId))
            {
                throw new EntityDuplicationException(nameof(ManufacturingOrder), manufacturingOrder.ManufacturingOrderId);
            }

            return _context.ManufacturingOrders
                .Add(manufacturingOrder)
                .Entity;
        }
        else
        {
            return manufacturingOrder;
        }
    }

    public async Task<ManufacturingOrder?> GetAsync(string manufacturingOrderId)
    {
        return await _context.ManufacturingOrders
            .Include(x => x.MaterialDefinition)
            .Include(x => x.OperationsDefinition)
                .ThenInclude(x => x.OperationsSegmentDependencies)
            .Include(x => x.OperationsDefinition)
                .ThenInclude(x => x.OperationsMaterialBill)
                    .ThenInclude(x => x.Items)
            .Include(x => x.OperationsDefinition)
                .ThenInclude(x => x.OperationsSegments)
                    .ThenInclude(x => x.MaterialSpecifications)
                        .ThenInclude(x => x.Properties)
            .Include(x => x.OperationsDefinition)
                .ThenInclude(x => x.OperationsSegments)
                    .ThenInclude(x => x.EquipmentSpecifications)
                        .ThenInclude(x => x.Properties)
            .Include(x => x.OperationsDefinition)
                .ThenInclude(x => x.OperationsSegments)
                    .ThenInclude(x => x.PersonnelSpecifications)
                        .ThenInclude(x => x.Properties)
            .Include(x => x.MaterialDefinition)
                .ThenInclude(x => x.MaterialClass)
            .Include(x => x.QualityStandard)
                .ThenInclude(x => x.Properties)
            .Include(x => x.CreatedBy)
            .Include(x => x.Customer)
            .Include(x => x.WorkOrders)
                .ThenInclude(x => x.OperationsSegment)
            .Include(x => x.WorkOrders)
                .ThenInclude(x => x.EquipmentWorkOrders)
            .Include(x => x.WorkOrders)
                .ThenInclude(x => x.PersonWorkOrders)
            .Include(x => x.WorkOrders)
                .ThenInclude(x => x.MaterialWorkOrders)
            .FirstOrDefaultAsync(x => x.ManufacturingOrderId == manufacturingOrderId);
    }


    public async Task<IEnumerable<ManufacturingOrder>> GetManufacturingOrders()
    {
        return await _context.ManufacturingOrders
            .AsNoTracking()
            .Include(x => x.OperationsDefinition)
            .Include(x => x.MaterialDefinition)
                .ThenInclude(x => x.MaterialClass)
            .Include(x => x.WorkOrders)
            .Include(x => x.QualityStandard)
                .ThenInclude(x => x.Properties)
            .Include(x => x.CreatedBy)
            .Include(x => x.Customer)
            .ToListAsync();
    }

    public async Task<List<ManufacturingOrder>> GetListByIdAsync(List<string> manufacturingOrderIds)
    {
        var manufacturingOrders = await _context.ManufacturingOrders
            .Include(x => x.MaterialDefinition)
            .ThenInclude(x => x.OperationsDefinitions)
            .Include(x => x.MaterialDefinition)
            .ThenInclude(x => x.MaterialClass)
            .Include(x => x.WorkOrders)
            .Where(x => manufacturingOrderIds.Contains(x.ManufacturingOrderId))
            .ToListAsync();

        var notFoundIds = manufacturingOrderIds
            .Where(id => !manufacturingOrders.Exists(pc => pc.ManufacturingOrderId == id));

        if (notFoundIds.Any())
        {
            throw new EntitiesNotFoundException(nameof(ManufacturingOrder), notFoundIds.ToList());
        }

        return manufacturingOrders;
    }

    public ManufacturingOrder Update(ManufacturingOrder manufacturingOrder)
    {
        return _context.ManufacturingOrders
                .Update(manufacturingOrder)
                .Entity;
    }

    public async Task<bool> ExistsAsync(string manufacturingOrderId)
    {
        return await _context.ManufacturingOrders
            .AnyAsync(x => x.ManufacturingOrderId == manufacturingOrderId);
    }

    public async Task DeleteAsync(string manufacturingOrderId)
    {
        var manufacturingOrder = await _context.ManufacturingOrders
            .FirstOrDefaultAsync(x => x.ManufacturingOrderId == manufacturingOrderId);

        if (manufacturingOrder is not null)
        {
            _context.ManufacturingOrders.Remove(manufacturingOrder);
        }
    }

    public async Task<ManufacturingOrder?> GetByIdAsync(string manufacturingOrderId)
    {
        return await _context.ManufacturingOrders
            .Include(x => x.OperationsDefinition)
                .ThenInclude(x => x.OperationsSegmentDependencies)
            .Include(x => x.OperationsDefinition)
                .ThenInclude(x => x.OperationsMaterialBill)
                    .ThenInclude(x => x.Items)
            .FirstOrDefaultAsync(x => x.ManufacturingOrderId == manufacturingOrderId);
    }

    public Task<List<ManufacturingOrder>> GetAll()
    {
        return _context.ManufacturingOrders
            .Include(x => x.WorkOrders)
            .ToListAsync();
    }

    public async Task UpdateStatusById(string manufacturingOrderId)
    {
        var manufacturingOrder = await _context.ManufacturingOrders
            .Include(x => x.WorkOrders)
            .FirstOrDefaultAsync(x => x.ManufacturingOrderId == manufacturingOrderId);
        if (manufacturingOrder is null) return;

        var workOrders = manufacturingOrder.WorkOrders;

        if (workOrders.Any(wo => wo.WorkOrderStatus == EStatus.Pending))
            manufacturingOrder.UpdateStatus(EStatus.Pending);
        else if (workOrders.Any(wo => wo.WorkOrderStatus == EStatus.PendingScheduling))
            manufacturingOrder.UpdateStatus(EStatus.PendingScheduling);
        else if (workOrders.Any(wo => wo.WorkOrderStatus == EStatus.InProduction))
            manufacturingOrder.UpdateStatus(EStatus.InProduction);
        else if (workOrders.Any(wo => wo.WorkOrderStatus == EStatus.ReadyForProduction))
            manufacturingOrder.UpdateStatus(EStatus.ReadyForProduction);
        else if (workOrders.All(wo => wo.WorkOrderStatus == EStatus.Completed))
            manufacturingOrder.UpdateStatus(EStatus.Completed);
        else
            manufacturingOrder.UpdateStatus(EStatus.Cancelled);

    }

    public Task<ManufacturingOrder> GetById(string moId)
    {
        return _context.ManufacturingOrders
            .Include(x => x.WorkOrders)
                .ThenInclude(x => x.PersonWorkOrders)
                    .ThenInclude(x => x.Person)
            .Include(x => x.WorkOrders)
                .ThenInclude(x => x.EquipmentWorkOrders)
                    .ThenInclude(x => x.Equipment)
            .Include(x => x.WorkOrders)
                .ThenInclude(x => x.MaterialWorkOrders)
                    .ThenInclude(x => x.MaterialDefinition)
            .FirstAsync(x => x.ManufacturingOrderId == moId);
    }

    public async Task UpdateDatesById(string manufacturingOrderId)
    {
        var mo = await _context.ManufacturingOrders
            .Include(x => x.WorkOrders)
            .FirstAsync(x => x.ManufacturingOrderId == manufacturingOrderId);

        var latestEndDate = mo.WorkOrders.Max(wo => wo.EndTime);
        var earliestStartDate = mo.WorkOrders.Min(wo => wo.StartTime);
        if (latestEndDate.HasValue && earliestStartDate.HasValue)
        {
            mo.UpdateDates(earliestStartDate.Value, latestEndDate.Value);
        }
    }

    public async Task<List<ManufacturingOrder>?> GetMOByTime(DateTime from, DateTime to)
    {
        return await _context.ManufacturingOrders
            .Include(x => x.MaterialDefinition)
                .ThenInclude(x => x.Properties)
            .Include(x => x.OperationsDefinition)
            .Include(x => x.WorkOrders)
                .ThenInclude(x => x.MaterialWorkOrders)
                    .ThenInclude(x => x.MaterialDefinition)
            .Where(mo => mo.CreateDate >= from && mo.CreateDate <= to)
            .ToListAsync();
    }
}
