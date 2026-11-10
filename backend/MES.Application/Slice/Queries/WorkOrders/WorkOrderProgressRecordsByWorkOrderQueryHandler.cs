namespace MesMicroservice.Application.Queries.WorkOrders;

public class WorkOrderProgressRecordsByWorkOrderQueryHandler : IRequestHandler<WorkOrderProgressRecordsByWorkOrderIdQuery, IEnumerable<WorkOrderProgressRecordDTO>>
{
    private readonly IWorkOrderProgressRecord _workOrderProgressRecord;
    public WorkOrderProgressRecordsByWorkOrderQueryHandler(IWorkOrderProgressRecord workOrderProgressRecord)
    {
        _workOrderProgressRecord = workOrderProgressRecord;
    }
    public async Task<IEnumerable<WorkOrderProgressRecordDTO>> Handle(WorkOrderProgressRecordsByWorkOrderIdQuery request, CancellationToken cancellationToken)
    {
        var workOrderProgressRecords = await _workOrderProgressRecord.GetByWorkOrderId(request.WorkOrderId);

        var result = workOrderProgressRecords.Select(record =>
            new WorkOrderProgressRecordDTO(
                id: record.Id,
                progressPercentage: record.ProgressPercentage,
                status: record.Status,
                totalQuantity: record.TotalQuantity,
                producedQuantity: record.ProducedQuantity,
                errorQuantity: record.ErrorQuantity,
                deltaQuantity: record.DeltaQuantity,
                deltaErrorQuantity: record.DeltaErrorQuantity,
                unitOfMeasure: record.UnitOfMeasure,
                timestamp: record.Timestamp,
                note: record.Note,
                equipment: record.Equipment,
                workOrderId: record.WorkOrderId,
                updatedBy: record.UpdatedBy
            )
        );

        return result;
    }
}
