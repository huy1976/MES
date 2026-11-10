namespace MesMicroservice.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WorkOrdersController : ApiControllerBase
{
    public WorkOrdersController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet("GetQuantityWorkOrders")]
    public async Task<IActionResult> GetQuantityWorkOrders([FromQuery] NumberWorkOrdersQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpPost("CreateListWorkOrders")]
    public async Task<IActionResult> CreateListWorkOrders([FromBody] CreateListWorkOrdersCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpPost("AddListWorkOrders")]
    public async Task<IActionResult> AddListWorkOrders([FromBody] AddListWorkOrdersCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpPatch("ConfirmListWorkOrders")]
    public async Task<IActionResult> ConfirmListWorkOrders([FromBody] ConfirmListWorkOrdersCommand command)
    {
        return await RequestAsync(command);
    }

    //[HttpPost("SplitWorkOrder")]
    //public async Task<IActionResult> SplitWorkOrder([FromBody] SplitWorkOrderCommand command)
    //{
    //    return await RequestAsync(command);
    //}

    [HttpGet("GetWorkOrders")]
    public async Task<IActionResult> GetWorkOrders([FromQuery] WorkOrdersQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpGet("GetWorkOrdersByStatus/{status}")]
    public async Task<IActionResult> GetWorkOrdersByStatus([FromRoute] EStatus status)
    {
        var query = new WorkOrderByStatusQuery(status);
        return await RequestAsync(query);
    }

    [HttpGet("WorkOrderByStatuses")]
    public async Task<IActionResult> GetWorkOrdersByStatuses([FromQuery] List<EStatus> statuses)
    {
        var query = new WorkOrderByStatusesQuery(statuses);
        return await RequestAsync(query);
    }


    [HttpGet("GetWorkOrderById/{woId}")]
    public async Task<IActionResult> GetWorkOrderById([FromRoute] string woId)
    {
        var query = new WorkOrderQuery(woId);
        return await RequestAsync(query);
    }

    [HttpPatch("UpdateWorkOrderStatus")]
    public async Task<IActionResult> UpdateWorkOrderStatus([FromBody] UpdateWorkOrderStatusCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpGet("WorkOrderProgressViewQuery")]
    public async Task<IActionResult> GetWorkOrderProgressViewQuery([FromQuery] WorkOrderProgressViewQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpPost("CreateWorkOrderProgressRecord")]
    public async Task<IActionResult> CreateWorkOrderProgressRecord([FromBody] CreateWorkOrderProgressRecordCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpPost("CreateWorkOrderErrorRecord")]
    public async Task<IActionResult> CreateWorkOrderErrorRecord([FromBody] CreateWorkOrderErrorRecordDTO dto)
    {


        switch (dto.DowntimeType)
        {
            case EDowntimeType.MinorStop:
                var command = new CreateWorkOrderErrorRecordByMinorStopCommand
                {
                    ErrorId = dto.ErrorId,
                    ErrorName = dto.ErrorName,
                    DowntimeType = dto.DowntimeType,
                    Notes = dto.Notes,
                    HandledBy = dto.HandledBy,
                    ReportedBy = dto.ReportedBy,
                    Start = dto.Start,
                    End = dto.End,
                    WorkOrderId = dto.WorkOrderId,
                    IsUpdateTime = dto.IsUpdateTime,
                    IsSchedule = dto.IsSchedule
                };
                return await RequestAsync(command);
            case EDowntimeType.MachineBreakdown:
                var majorStopCommand = new CreateWorkOrderErrorRecordByMachineBreakdownCommand
                {
                    ErrorId = dto.ErrorId,
                    ErrorName = dto.ErrorName,
                    DowntimeType = dto.DowntimeType,
                    Notes = dto.Notes,
                    HandledBy = dto.HandledBy,
                    ReportedBy = dto.ReportedBy,
                    Start = dto.Start,
                    End = dto.End,
                    WorkOrderId = dto.WorkOrderId,
                    IsUpdateTime = dto.IsUpdateTime,
                    IsSchedule = dto.IsSchedule
                };
                return await RequestAsync(majorStopCommand);
            case EDowntimeType.MajorMachineBreakdown:
                var breakdownCommand = new CreateWorkOrderErrorRecordByMinorStopCommand
                {
                    ErrorId = dto.ErrorId,
                    ErrorName = dto.ErrorName,
                    DowntimeType = dto.DowntimeType,
                    Notes = dto.Notes,
                    HandledBy = dto.HandledBy,
                    ReportedBy = dto.ReportedBy,
                    Start = dto.Start,
                    End = dto.End,
                    WorkOrderId = dto.WorkOrderId,
                    IsUpdateTime = dto.IsUpdateTime,
                    IsSchedule = dto.IsSchedule
                };
                return await RequestAsync(breakdownCommand);
            default:
                return BadRequest("Invalid DowntimeType");
        }

    }

    [HttpGet("ReviewNewScheduleTimeForWo")]
    public async Task<IActionResult> CreateWorkOrderErrorRecord([FromQuery] ReviewNewScheduleTimeForWoQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpGet("GetWorkOrderProgressRecordsByWorkOrderId/{workOrderId}")]
    public async Task<IActionResult> GetWorkOrderProgressRecordsByWorkOrderId([FromRoute] string workOrderId)
    {
        var query = new WorkOrderProgressRecordsByWorkOrderIdQuery(workOrderId);

        return await RequestAsync(query);
    }

    [HttpGet("GetWorkOrderErrorRecordsByWorkOrderId/{workOrderId}")]
    public async Task<IActionResult> GetWorkOrderErrorRecordsByWorkOrderId([FromRoute] string workOrderId)
    {
        var query = new WorkOrderErrorRecordsByWorkOrderIdQuery(workOrderId);

        return await RequestAsync(query);
    }


    //[HttpPost]
    //[Route("{manufacturingOrderId}")]
    //public async Task<IActionResult> CreateWorkOrder([FromRoute] string manufacturingOrderId, [FromBody] CreateWorkOrderViewModel workOrder)
    //{
    //    var command = new CreateWorkOrderCommand(manufacturingOrderId, workOrder.WorkOrderId, workOrder.DueDate, workOrder.StartTime, workOrder.EndTime, workOrder.WorkOrderStatus, workOrder.PrerequisiteOperations, workOrder.WorkCenter, workOrder.EquipmentRequirements);
    //    return await RequestAsync(command);
    //}

    //[HttpPut]
    //[Route("{manufacturingOrderId}/{workOrderId}")]
    //public async Task<IActionResult> UpdateWorkOrder([FromRoute] string manufacturingOrderId, [FromRoute] string workOrderId, [FromBody] UpdateWorkOrderViewModel workOrder)
    //{
    //    var command = new UpdateWorkOrderCommand(manufacturingOrderId, workOrderId, workOrder.StartTime, workOrder.EndTime, workOrder.ActuallyStartTime, workOrder.ActuallyEndTime, workOrder.WorkOrderStatus, workOrder.WorkCenter);
    //    return await RequestAsync(command);
    //}

    //[HttpGet]
    //public async Task<IEnumrable<WorkOrderViewModel>> GetWorkOrders([FromQuery] WorkOrdersQuery query)
    //{
    //    return await _mediator.Send(query);
    //}

    //[HttpGet]
    //[Route("{manufacturingOrderId}/{workOrderId}")]
    //public async Task<IActionResult> GetWorkOrder([FromRoute] string manufacturingOrderId, [FromRoute] string workOrderId)
    //{
    //    var query = new WorkOrderQuery(manufacturingOrderId, workOrderId);
    //    var result = await _mediator.Send(query);

    //    if (result is null)
    //    {
    //        return NotFound();
    //    }
    //    else
    //    {
    //        return Ok(result);
    //    }
    //}

    //[HttpDelete]
    //[Route("{manufacturingOrderId}/{workOrderId}")]
    //public async Task<IActionResult> DeleteWorkOrder([FromRoute] string manufacturingOrderId, [FromRoute] string workOrderId)
    //{
    //    var command = new DeleteWorkOrderCommand(manufacturingOrderId, workOrderId);
    //    return await RequestAsync(command);
    //}
}
