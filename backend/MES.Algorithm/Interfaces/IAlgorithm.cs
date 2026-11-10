global using MesMicroservice.Algorithm.Models;

namespace MesMicroservice.Algorithm.Interfaces
{
    public interface IAlgorithm
    {
        Task<Solution> Run(Schedule schedule);

    }
}
