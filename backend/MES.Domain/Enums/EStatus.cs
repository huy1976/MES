namespace MesMicroservice.Domain.Enums
{
    public enum EStatus
    {
        Active = 0,
        Pending = 1,
        PendingScheduling = 2,
        Planned = 3,
        ReadyForProduction = 4,
        InProduction = 5,
        Completed = 6,
        Cancelled = 7,
        Available = 8,
        Running = 9,
        Breakdown = 10,
        Maintenance = 11,
        Pause = 12,

    }
}
