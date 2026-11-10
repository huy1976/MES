namespace MesMicroservice.Domain.Enums
{
    public enum EDowntimeType
    {
        MinorStop, // <= 5ph
        MachineBreakdown, // > 5ph and <= 30ph
        MajorMachineBreakdown // > 30ph
    }

}
