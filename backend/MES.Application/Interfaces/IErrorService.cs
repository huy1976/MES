namespace MesMicroservice.Application.Interfaces
{
    public interface IErrorService
    {
        List<ReScheduleTimeForWODTO> UpdateTimeStamp(WorkOrder currentW, List<WorkOrder> workOrders, double duration);
        void Scheduling();

        List<ReScheduleTimeForWODTO> RescheduleAfterMajorMachineBreakdown(WorkOrder brokenWorkOrder, List<WorkOrder> candidateWorkOrders,double downtimeHours);

    }
}
