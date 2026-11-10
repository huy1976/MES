global using MesMicroservice.Algorithm.Heuristics.InitialSchedules;
global using MesMicroservice.Algorithm.Metaheuristics.Improvement.TabuSearch;
using MesMicroservice.Algorithm.BruteForces;
using MesMicroservice.Algorithm.Metaheuristics.Improvement.GeneticAlgorithm;
using Microsoft.Extensions.DependencyInjection;

namespace MesMicroservice.Algorithm
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAlgorithmServices(this IServiceCollection services)
        {

            // Heuristics
            services.AddTransient<IHeuristic, GuptaDarrow1>();
            services.AddTransient<IHeuristic, InsertionMethod>();
            services.AddTransient<IHeuristic, FIFO>();
            services.AddTransient<IHeuristic, EDD>();
            services.AddTransient<IHeuristicFactory, HeuristicFactory>();
            services.AddTransient<GuptaDarrow1>();
            services.AddTransient<InsertionMethod>();
            services.AddTransient<FIFO>();
            services.AddTransient<EDD>();

            // Assignment
            services.AddTransient<IAssignmentMethod, Assignment>();

            // Metaheuristics
            services.AddTransient<TabuSearchService>();
            services.AddTransient<GAService>();
            // BruteForce
            services.AddTransient<BruteForce>();

            // Core services
            services.AddTransient<IEquipmentSelectorService, EquipmentSelectorService>();
            services.AddTransient<IMachineTimeCalculator, MachineTimeCalculator>();
            services.AddTransient<IPersonnelSelectorService, PersonnelSelectorService>();


            return services;
        }
    }
}
