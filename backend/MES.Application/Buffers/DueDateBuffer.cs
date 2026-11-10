namespace MesMicroservice.Application.Buffers;

public class DueDateBuffer
{
    private readonly Dictionary<string, DateTime> maxPrerequisiteDueDate = new();

    public void Add(string workOrderId)
    {
        maxPrerequisiteDueDate.Add(workOrderId, DateTime.MaxValue);
    }

    public void Update(string workOrderId, DateTime dueDate)
    {
        maxPrerequisiteDueDate[workOrderId] = dueDate;
    }

    public void Remove(string workOrderId)
    {
        maxPrerequisiteDueDate.Remove(workOrderId);
    }

    public DateTime GetPrereqDueDate(string workOrderId)
    {
        return maxPrerequisiteDueDate[workOrderId];
    }
}
