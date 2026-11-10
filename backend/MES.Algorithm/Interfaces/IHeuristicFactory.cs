using MesMicroservice.Algorithm.Models.Enum;

namespace MesMicroservice.Algorithm.Interfaces
{
    public interface IHeuristicFactory
    {
        IHeuristic Create(HeuristicType heuristic);
    }
}
