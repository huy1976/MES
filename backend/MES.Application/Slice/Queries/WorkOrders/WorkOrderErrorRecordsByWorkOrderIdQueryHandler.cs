
using System.Linq;

namespace MesMicroservice.Application.Queries.WorkOrders;

public class WorkOrderErrorRecordsByWorkOrderIdQueryHandler : IRequestHandler<WorkOrderErrorRecordsByWorkOrderIdQuery, IEnumerable<WorkOrderErrorRecordDTO>>
{
    private readonly IWorkOrderErrorRecord _workOrderErrorRecord;
    public WorkOrderErrorRecordsByWorkOrderIdQueryHandler(IWorkOrderErrorRecord workOrderErrorRecord)
    {
        _workOrderErrorRecord = workOrderErrorRecord;
    }
    public async Task<IEnumerable<WorkOrderErrorRecordDTO>> Handle(WorkOrderErrorRecordsByWorkOrderIdQuery request, CancellationToken cancellationToken)
    {
        var workOrderErrorRecords = await _workOrderErrorRecord.GetByWorkOrderId(request.WorkOrderId);

        var result = workOrderErrorRecords.Select(record =>
            new WorkOrderErrorRecordDTO(
                    id: record.Id,
                    errorId: record.ErrorId,
                    errorName: record.ErrorName,
                    status: record.Status,
                    downtimeType: ConvertStatusToString(record.DowntimeType),
                    notes: record.Notes,
                    handledBy: record.HandledBy,
                    reportedBy: record.ReportedBy,
                    timestamp: record.Timestamp,
                    durationInHours: record.DurationInHours,
                    start: record.Start,
                    end: record.End,
                    workOrderId: record.WorkOrderId,
                    isUpdateTime: record.IsUpdateTime,
                    isSchedule: record.IsSchedule
                )
        );
        return result;
    }

    private string ConvertStatusToString(EDowntimeType downtimeType)
    {
        return downtimeType switch
        {
            EDowntimeType.MinorStop => "Ảnh hưởng ít",
            EDowntimeType.MachineBreakdown => "Ảnh hưởng tưởng đối",
            EDowntimeType.MajorMachineBreakdown => "Ảnh hưởng nhiều",
            _ => "Unknown"
        };
    }

}
