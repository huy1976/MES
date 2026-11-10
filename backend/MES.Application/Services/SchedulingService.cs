global using MesMicroservice.Algorithm.Heuristics.InitialSchedules;
global using MesMicroservice.Algorithm.Interfaces;
global using MesMicroservice.Algorithm.Metaheuristics.Improvement.GeneticAlgorithm;
global using MesMicroservice.Algorithm.Metaheuristics.Improvement.TabuSearch;
global using Microsoft.Extensions.DependencyInjection;
using MesMicroservice.Algorithm.BruteForces;
using MesMicroservice.Algorithm.Services;
using MesMicroservice.MachineLearning; // Thêm namespace của AI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SchedulingService : ISchedulingService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TabuAIPredictor _aiPredictor; // 1. Khai báo AI Predictor

    // 2. Inject AI Predictor vào Constructor
    public SchedulingService(IServiceProvider serviceProvider, TabuAIPredictor aiPredictor)
    {
        _serviceProvider = serviceProvider;
        _aiPredictor = aiPredictor;
    }

    public async Task<Solution> GenerateScheduleAsync(List<Job> jobs, List<Machine> machines, string algorithmType, bool useAI = false)
    {
        var jobDict = jobs.GroupBy(j => j.ManufacturingOrderId)
                          .ToDictionary(g => g.Key, g => g.ToList());

        var schedule = new Schedule(machines, jobDict);

        if (algorithmType == "TabuSearch")
        {
            var tabuService = _serviceProvider.GetRequiredService<TabuSearchService>();
            var tabuSettings = new TabuSearchSettings
            {
                UseAI = false,
                RTS = true,
                MaxIterations = 2000
            };

            if (useAI)
            {
                var features = AIFeatureExtractor.Extract(jobs, machines);
                var (aiTenure, aiNeighbors) = _aiPredictor.PredictBestParameters(features);

                tabuSettings.TabuTenure = Math.Clamp(aiTenure, 3, 80);
                tabuSettings.NumNeighborsPerMoAndStage = Math.Clamp(aiNeighbors, 20, 500);
            }
            else
            {
                tabuSettings.TabuTenure = 10;
                tabuSettings.NumNeighborsPerMoAndStage = 20;
            }

            return await tabuService.RunWithSettings(schedule, tabuSettings);
        }


        // CÁC THUẬT TOÁN KHÁC GIỮ NGUYÊN...
        IAlgorithm selectedAlgorithm = algorithmType switch
        {
            "GuptaDarrow1" => _serviceProvider.GetRequiredService<GuptaDarrow1>(),
            "InsertionMethod" => _serviceProvider.GetRequiredService<InsertionMethod>(),
            "GeneticAlgorithm" => _serviceProvider.GetRequiredService<GAService>(),
            "BruteForce" => _serviceProvider.GetRequiredService<BruteForce>(),
            "FIFO" => _serviceProvider.GetRequiredService<FIFO>(),
            "EDD" => _serviceProvider.GetRequiredService<EDD>(),
            _ => _serviceProvider.GetRequiredService<TabuSearchService>()
        };

        return await selectedAlgorithm.Run(schedule);
    }

    // --- HÀM HELPER: Lấy Features ---
    private TabuAITrainingData ExtractAIFeatures(List<Job> jobs, List<Machine> machines)
    {
        // Bạn copy đoạn code tính toán Features (SetupConflictRatio, SpeedStd,...) 
        // từ file MLDataGeneratorController.cs xuống đây.
        // Chỉ cần truyền list Jobs và Machines vào là tính được ngay.

        return new TabuAITrainingData { TotalJobs = jobs.Count /*, ... gán nốt các biến khác */ };
    }
}