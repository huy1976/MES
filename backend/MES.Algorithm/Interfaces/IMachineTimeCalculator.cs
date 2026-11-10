namespace MesMicroservice.Algorithm.Interfaces
{
    public interface IMachineTimeCalculator
    {
        void CalculateProcessingTimes(Dictionary<string, List<Job>> jobs);
        double CalculateProcessingTimeForMachine(Job job, Machine machine);
        double SetupTime(Job? from, Job? to, int stage);

    }
}
