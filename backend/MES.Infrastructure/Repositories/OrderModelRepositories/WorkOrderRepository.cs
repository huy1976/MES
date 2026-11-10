
using MesMicroservice.Domain.AggregateModels.WorkOrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace MesMicroservice.Infrastructure.Repositories.OrderModelRepositories;

public class WorkOrderRepository : BaseRepository, IWorkOrderRepository
{
    public WorkOrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<WorkOrder> Add(int manufacturingOrderId, WorkOrder workOrder)
    {
        if (workOrder.IsTransient())
        {
            if (await ExistsAsync(manufacturingOrderId, workOrder.WorkOrderId))
            {
                throw new EntityDuplicationException(nameof(WorkOrder), workOrder.WorkOrderId);
            }

            return _context.WorkOrders
                .Add(workOrder)
                .Entity;
        }
        else
        {
            return workOrder;
        }
    }
    public async Task AddAsync(WorkOrder workOrder)
    {
        await _context.WorkOrders.AddAsync(workOrder);
    }
    public async Task<WorkOrder?> GetAsync(int manufacturingOrderId, string workOrderId)
    {
        return await _context.WorkOrders
            .AsNoTracking()
            .Include(x => x.MaterialDefinition) 
            .Include(x => x.OperationsSegment) 
            .Include(x => x.ManufacturingOrder)
                .ThenInclude(x => x.QualityStandard)
            .FirstOrDefaultAsync(x => x.ManufacturingOrderId == manufacturingOrderId && x.WorkOrderId == workOrderId);
    }


    public async Task<List<WorkOrder>> GetManufacturingOrderWorkOrderByIdsAsync(int manufacturingOrderId, List<string> workOrderIds)
    {
        var workOrders = await _context.WorkOrders
            .Include(x => x.ManufacturingOrder)
            .Include(x => x.WorkCenter)
            .Include(x => x.ManufacturingRecords)
            .Where(x => x.ManufacturingOrderId == manufacturingOrderId && workOrderIds.Contains(x.WorkOrderId))
            .ToListAsync();

        var notFoundIds = workOrderIds
            .Where(id => !workOrders.Exists(pc => pc.WorkOrderId == id));

        if (notFoundIds.Any())
        {
            throw new EntitiesNotFoundException(nameof(WorkOrder), notFoundIds.ToList());
        }

        return workOrders;
    }

    public WorkOrder Update(WorkOrder workOrder)
    {
        return _context.WorkOrders
                .Update(workOrder)
                .Entity;
    }

    public async Task<bool> ExistsAsync(int manufacturingOrderId, string workOrderId)
    {
        return await _context.WorkOrders
            .AnyAsync(x => x.ManufacturingOrderId == manufacturingOrderId && x.WorkOrderId == workOrderId);
    }

    public async Task DeleteAsync(int manufacturingOrderId, string workOrderId)
    {
        var workOrder = await _context.WorkOrders
            .FirstOrDefaultAsync(x => x.ManufacturingOrderId == manufacturingOrderId && x.WorkOrderId == workOrderId);

        if (workOrder is not null)
        {
            _context.WorkOrders.Remove(workOrder);
        }
    }

    public async Task<List<WorkOrder>> GetListByIds(List<(string ManufacturingOrderId, string WorkOrderId)> ids)
    {
        var idStrings = ids.Select(id => $"{id.ManufacturingOrderId} - {id.WorkOrderId}").ToList();

        var workOrders = await _context.WorkOrders
            .Include(x => x.ManufacturingOrder)
            .Include(x => x.WorkCenter)
            .Include(x => x.ManufacturingRecords)
            .ThenInclude(x => x.Equipments)
            .Where(x => idStrings.Contains(x.ManufacturingOrder.ManufacturingOrderId + " - " + x.WorkOrderId))
            .ToListAsync();

        return workOrders;
    }

    public async Task<List<WorkOrder>> GetListByStatus(EStatus workOrderStatus)
    {
        var workOrders = await _context.WorkOrders
            .Include(x => x.ManufacturingOrder)
            .Include(x => x.WorkCenter)
            .Include(x => x.ManufacturingRecords)
            .ThenInclude(x => x.Equipments)
            .Where(x => x.WorkOrderStatus == workOrderStatus)
            .ToListAsync();

        return workOrders;
    }

    public async Task<int> GetQuantityWorkOrders()
    {
        return await _context.WorkOrders.CountAsync();
    }

    public async Task<List<WorkOrder>> GetByIds(List<int> ids)
    {
        var workOrders = await _context.WorkOrders
            .AsNoTracking() 
            .Where(x => ids.Contains(x.Id))
            .Include(x => x.MaterialDefinition)
            .Include(x => x.OperationsSegment)
                .ThenInclude(x => x.MaterialSpecifications) 
            .Include(x => x.ManufacturingOrder)
            .Include(x => x.OperationsSegment)
                .ThenInclude(x => x.OperationsDefinition) 
                    .ThenInclude(y => y.OperationsMaterialBill)
                        .ThenInclude(z => z.Items)
            .ToListAsync();

        return workOrders;
    }
    public async Task<List<WorkOrder>> GetByWorkOrderCodesAsync(List<string> workOrderCodes)
    {
        return await _context.WorkOrders
            .AsNoTracking()
            .Include(x => x.MaterialDefinition).ThenInclude(x => x.Properties) // Lấy thuộc tính sản phẩm
            .Include(x => x.OperationsSegment).ThenInclude(x => x.MaterialSpecifications) // Lấy công đoạn và vật tư
            .Include(x => x.ManufacturingOrder) // Lấy mã đơn hàng sản xuất
            .Where(w => workOrderCodes.Contains(w.WorkOrderId))
            .ToListAsync();
    }

    public async Task<WorkOrder> GetById(int id)
    {
        var workOrder = await _context.WorkOrders
            .AsNoTracking()
            .Include(x => x.MaterialDefinition)
            .Include(x => x.OperationsSegment)
                .ThenInclude(x => x.EquipmentSpecifications)
                    .ThenInclude(y => y.Properties)
            .Include(x => x.OperationsSegment)
                .ThenInclude(x => x.MaterialSpecifications)
                    .ThenInclude(y => y.Properties)
            .Include(x => x.OperationsSegment)
                .ThenInclude(x => x.PersonnelSpecifications)
                    .ThenInclude(y => y.Properties)
            .Include(x => x.ManufacturingOrder)
            .Include(x => x.WorkCenter)
            .Include(x => x.ManufacturingRecords)
            .ThenInclude(x => x.Equipments)
        .FirstOrDefaultAsync(x => x.Id== id);

        return workOrder;

    }

    public Task<bool> ExistsAsync(string workOrderId)
    {
        return _context.WorkOrders
            .AnyAsync(x => x.WorkOrderId == workOrderId);
    }

    public async Task<List<WorkOrder>> GetByAssignedEquipmentId(string equipmentId)
    {
        var workOrders = await _context.WorkOrders
            .AsNoTracking()
            .Include(x => x.MaterialDefinition)
                .ThenInclude(md => md.Properties)
            .Include(x => x.OperationsSegment)
                .ThenInclude(os => os.MaterialSpecifications)
            .Include(w => w.EquipmentWorkOrders)
                .ThenInclude(e => e.Equipment)
            .Include(x => x.ManufacturingOrder)
            .Where(w => w.EquipmentWorkOrders
                .Any(e => e.Equipment.ResourceId == equipmentId))
            .ToListAsync();

        return workOrders;
    }

    public async Task<IEnumerable<WorkOrder>> GetAll()
    {
        var workOrders = await _context.WorkOrders
            .AsNoTracking()
            .Include(x => x.MaterialDefinition)
                .ThenInclude(md => md.Properties)
            .Include(x => x.OperationsSegment)
                .ThenInclude(os => os.MaterialSpecifications)
            .Include(x => x.OperationsSegment) 
            .Include(x => x.ManufacturingOrder)
                .ThenInclude(mo => mo.QualityStandard)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return workOrders;
    }

    public async Task<List<WorkOrder>> GetByWoIds(List<string> ids)
    {
        var workOrders = await _context.WorkOrders
            .Where(x => ids.Contains(x.WorkOrderId))
            .Include(x => x.MaterialDefinition)
            .Include(x => x.OperationsSegment)
                .ThenInclude(x => x.MaterialSpecifications)
            .Include(x => x.ManufacturingOrder)
            .Include(x => x.OperationsSegment)
                .ThenInclude(x => x.OperationsDefinition)
                    .ThenInclude(y => y.OperationsMaterialBill)
                        .ThenInclude(z => z.Items)
            .ToListAsync();

        return workOrders;
    }

    public async Task<WorkOrder?> GetByWoIdAsync(string workOrderId)
    {
        return await _context.WorkOrders
            .Include(x => x.EquipmentWorkOrders)
                .ThenInclude(x => x.Equipment)
            .Include(x => x.PersonWorkOrders)
                .ThenInclude(x => x.Person)
            .Include(x => x.MaterialWorkOrders)
                .ThenInclude(x => x.MaterialDefinition)
            .Include(x => x.MaterialDefinition)
            .Include(x => x.OperationsSegment)
            .Include(x => x.ManufacturingOrder)
                .ThenInclude(x => x.QualityStandard)
            .FirstOrDefaultAsync(x => x.WorkOrderId == workOrderId);
    }

    public async Task<WorkOrder?> GetListBomByWoIdAsync(string workOrderId)
    {
        var workOrder = await _context.WorkOrders
            .AsTracking()
            .Include(x => x.OperationsSegment)
                .ThenInclude(x => x.OperationsDefinition)
                    .ThenInclude(y => y.OperationsMaterialBill)
                        .ThenInclude(z => z.Items)
            .Include(x => x.OperationsSegment)
                .ThenInclude(x => x.MaterialSpecifications)
            .FirstOrDefaultAsync(x => x.WorkOrderId == workOrderId);

        return workOrder;
    }

    public async Task<List<WorkOrder>?> GetWOByTime(DateTime from, DateTime to)
    {
        var wo =  await _context.WorkOrders
            .Include(x => x.MaterialWorkOrders)
                .ThenInclude(mw => mw.MaterialDefinition)
            .Include(x => x.PersonWorkOrders)
                .ThenInclude(pw => pw.Person)
            .Where(wo => wo.StartTime <= to && wo.EndTime >= from)
            .ToListAsync();

        return wo;
    }

    public async Task<List<WorkOrder>> GetAlAsync()
    {
        var workOrders = await _context.WorkOrders
            .AsTracking()
            .Include(x => x.EquipmentWorkOrders)
                .ThenInclude(x => x.Equipment)
            .ToListAsync();

        return workOrders;
    }

    public async Task<List<WorkOrder>> GetByAssignedPersonId(string personId)
    {
        var workOrders = await _context.WorkOrders
            .AsNoTracking()
            .Include(x => x.MaterialDefinition)
                .ThenInclude(md => md.Properties)
            .Include(x => x.OperationsSegment)
                .ThenInclude(os => os.MaterialSpecifications)
            .Include(w => w.EquipmentWorkOrders)
                .ThenInclude(e => e.Equipment)
            .Include(x => x.ManufacturingOrder)
            .Where(w => w.PersonWorkOrders
                .Any(e => e.Person.ResourceId == personId))
            .ToListAsync();

        return workOrders;
    }
}
