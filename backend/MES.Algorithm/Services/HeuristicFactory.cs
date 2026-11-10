global using MesMicroservice.Algorithm.Models.Enum;
global using MesMicroservice.Algorithm.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MesMicroservice.Algorithm.Services
{
    public class HeuristicFactory : IHeuristicFactory
    {
        private IServiceProvider _serviceProvider;

        public HeuristicFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IHeuristic Create(HeuristicType heuristic)
        {
            return heuristic switch
            {
                HeuristicType.GuptaDarrow1 => _serviceProvider.GetRequiredService<GuptaDarrow1>(),
                HeuristicType.InsertionMethod => _serviceProvider.GetRequiredService<InsertionMethod>(),
                _ => throw new ArgumentException($"Unknown metaheuristic type: {heuristic}")
            };
        }
    }
}
