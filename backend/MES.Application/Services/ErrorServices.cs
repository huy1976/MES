namespace MesMicroservice.Application.Services;

public class ErrorServices : IErrorService
{
    private List<WorkOrder> GetAffectedWorkOrders(WorkOrder currentWo, List<WorkOrder> allWorkOrders)
    {
        var result = new List<WorkOrder>();
        var visited = new HashSet<int>();
        var queue = new Queue<WorkOrder>();

        queue.Enqueue(currentWo);
        visited.Add(currentWo.Id);

        while (queue.Any())
        {
            var wo = queue.Dequeue();

            // 1. Các lệnh chạy sau trên cùng máy
            var nextOnSameMachine = allWorkOrders.Where(w =>
                w.Id != wo.Id &&
                w.WorkOrderStatus == EStatus.ReadyForProduction &&
                w.StartTime.HasValue &&
                wo.StartTime.HasValue &&
                w.StartTime > wo.StartTime &&
                w.EquipmentWorkOrders.Any(eq =>
                    wo.EquipmentWorkOrders.Any(e =>
                        e.Equipment.ResourceId == eq.Equipment.ResourceId))
            );

            foreach (var next in nextOnSameMachine)
            {
                if (visited.Add(next.Id))
                {
                    result.Add(next);
                    queue.Enqueue(next);
                }
            }

            // 2. Các lệnh chờ phía sau trong cùng MO
            var nextInSameMO = allWorkOrders.Where(w =>
                w.Id != wo.Id &&
                w.ManufacturingOrderId == wo.ManufacturingOrderId &&
                w.WorkOrderStatus == EStatus.ReadyForProduction &&
                w.SequenceNumber > wo.SequenceNumber
            );

            foreach (var next in nextInSameMO)
            {
                if (visited.Add(next.Id))
                {
                    result.Add(next);
                    queue.Enqueue(next);
                }
            }
        }

        return result;
    }


    public List<ReScheduleTimeForWODTO> UpdateTimeStamp(WorkOrder currentWo, List<WorkOrder> workOrders, double duration)
    {
        var result = new List<ReScheduleTimeForWODTO>();
        var affectedWOs = GetAffectedWorkOrders(currentWo, workOrders);

        foreach (var wo in affectedWOs
            .OrderBy(w => w.StartTime))
        {
            var oldStart = wo.StartTime ?? DateTime.MinValue;
            var oldEnd = wo.EndTime ?? DateTime.MinValue;

            wo.Delay(TimeSpan.FromHours(duration));

            result.Add(new ReScheduleTimeForWODTO
            {
                WorkOrderId = wo.WorkOrderId,
                NewStart = wo.StartTime ?? DateTime.MinValue,
                NewEnd = wo.EndTime ?? DateTime.MinValue,
                OldStart = oldStart,
                OldEnd = oldEnd
            });
        }

        return result;
    }

    public void Scheduling()
    {
        throw new NotImplementedException();
    }

    public List<ReScheduleTimeForWODTO> RescheduleAfterMajorMachineBreakdown(WorkOrder brokenWorkOrder, List<WorkOrder> candidateWorkOrders, double downtimeHours)
    {
        var result = new List<ReScheduleTimeForWODTO>();

        if (brokenWorkOrder == null)
        {
            return result;
        }

        if (downtimeHours <= 0)
        {
            return result;
        }

        if (!brokenWorkOrder.StartTime.HasValue || !brokenWorkOrder.EndTime.HasValue)
        {
            return result;
        }

        /*
            Lấy danh sách máy của WO bị sự cố.
            Chỉ dời các WO phía sau nếu chúng dùng chung máy.
        */
        var brokenEquipmentIds = brokenWorkOrder.EquipmentWorkOrders?
            .Select(x => x.Equipment?.ResourceId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet()
            ?? new HashSet<string?>();

        var currentEndTime = brokenWorkOrder.EndTime.Value;

        /*
            Chỉ xét các WO có thời gian bắt đầu sau WO bị sự cố.
            Không xét WO đã completed.
        */
        var affectedWorkOrders = candidateWorkOrders
            .Where(x =>
                x.Id != brokenWorkOrder.Id &&
                x.StartTime.HasValue &&
                x.EndTime.HasValue &&
                x.StartTime.Value >= brokenWorkOrder.StartTime.Value)
            .OrderBy(x => x.StartTime!.Value)
            .ToList();

        foreach (var wo in affectedWorkOrders)
        {
            var woEquipmentIds = wo.EquipmentWorkOrders?
                .Select(x => x.Equipment?.ResourceId)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet()
                ?? new HashSet<string?>();

            var isSameEquipment = brokenEquipmentIds.Count == 0 ||
                                  woEquipmentIds.Count == 0 ||
                                  woEquipmentIds.Overlaps(brokenEquipmentIds);

            if (!isSameEquipment)
            {
                continue;
            }

            var oldStart = wo.StartTime!.Value;
            var oldEnd = wo.EndTime!.Value;

            var duration = oldEnd - oldStart;

            /*
                Nếu WO phía sau bị chồng với thời gian kết thúc mới,
                dời nó bắt đầu sau currentEndTime.
                Nếu không chồng, vẫn có thể giữ nguyên.
            */
            var newStart = oldStart < currentEndTime
                ? currentEndTime
                : oldStart;

            var newEnd = newStart.Add(duration);

            wo.UpdatePlanTime(newStart, newEnd);

            currentEndTime = newEnd;

            result.Add(new ReScheduleTimeForWODTO
            {
                WorkOrderId = wo.WorkOrderId,
                OldStart = oldStart,
                OldEnd = oldEnd,
                NewStart = newStart,
                NewEnd = newEnd
            });
        }

        return result;
    }
}
