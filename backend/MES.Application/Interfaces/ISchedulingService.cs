global using MesMicroservice.Algorithm.Models;

namespace MesMicroservice.Application.Interfaces
{
    public interface ISchedulingService
    {
        Task<Solution> GenerateScheduleAsync(List<Job> jobs, List<Machine> machines, string algorithmType, bool useAI = false);
    }
}
