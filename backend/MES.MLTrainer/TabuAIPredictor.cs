using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;
using System.Linq;

namespace MesMicroservice.MachineLearning
{
    public class TabuAIPredictor
    {
        private static readonly object _aiLock = new object();
        private readonly MLContext _mlContext;
        private readonly PredictionEngine<TabuAITrainingData, ObjectivePrediction> _objectiveEngine;

        public TabuAIPredictor()
        {
            _mlContext = new MLContext(seed: 42);

            string baseDir = AppContext.BaseDirectory;

            string? objectiveModelPath = Directory.GetFiles(baseDir, "TabuObjectiveModel*.zip")
                .OrderByDescending(File.GetLastWriteTime)
                .FirstOrDefault();

            if (objectiveModelPath == null)
                throw new FileNotFoundException($"Không tìm thấy file model TabuObjectiveModel*.zip trong {baseDir}");

            ITransformer objectiveModel = _mlContext.Model.Load(objectiveModelPath, out _);
            _objectiveEngine = _mlContext.Model.CreatePredictionEngine<TabuAITrainingData, ObjectivePrediction>(objectiveModel);

            Console.WriteLine($"[AI Init] Loaded model: {Path.GetFileName(objectiveModelPath)}");
        }

        public (int optimalTenure, int optimalNeighbors) PredictBestParameters(TabuAITrainingData baseInput)
        {
            if (baseInput == null) throw new ArgumentNullException(nameof(baseInput));

            int totalJobs = Math.Max(1, (int)Math.Round(baseInput.TotalJobs));

            var candidateTenures = new[] { 5, 8, 11, 15, 20, 25, 30, 35, 40, 50, 60, 80 }
                .Where(t => t <= Math.Max(8, totalJobs * 2))
                .ToArray();

            var candidateNeighbors = new[] { 50, 80, 120, 160, 200, 250, 300, 350, 400, 500 }
                .Where(n => n <= Math.Max(80, totalJobs * 8))
                .ToArray();

            if (!candidateTenures.Any())
                candidateTenures = new[] { 5, 8, 11 };

            if (!candidateNeighbors.Any())
                candidateNeighbors = new[] { 50, 80 };

            double bestScore = double.MaxValue;
            int bestTenure = candidateTenures[0];
            int bestNeighbors = candidateNeighbors[0];

            lock (_aiLock)
            {
                foreach (var tenure in candidateTenures)
                {
                    foreach (var neighbors in candidateNeighbors)
                    {
                        var input = CloneInput(baseInput);
                        input.Tenure = tenure;
                        input.Neighbors = neighbors;

                        var prediction = _objectiveEngine.Predict(input);

                        if (prediction.Score < bestScore)
                        {
                            bestScore = prediction.Score;
                            bestTenure = tenure;
                            bestNeighbors = neighbors;
                        }
                    }
                }
            }

            Console.WriteLine($"[AI Predict] T={bestTenure}, N={bestNeighbors}, predictedRelativeObjective={bestScore:F4}");

            return (bestTenure, bestNeighbors);
        }

        private static TabuAITrainingData CloneInput(TabuAITrainingData x)
        {
            return new TabuAITrainingData
            {
                TotalJobs = x.TotalJobs,
                MachineCount = x.MachineCount,
                AvgEligibleMachines = x.AvgEligibleMachines,
                MinEligibleMachines = x.MinEligibleMachines,
                MaxEligibleMachines = x.MaxEligibleMachines,
                AvgProcessingTime = x.AvgProcessingTime,
                StdProcessingTime = x.StdProcessingTime,
                AvgDiameter = x.AvgDiameter,
                DiameterVariance = x.DiameterVariance,
                AvgSetupTime = x.AvgSetupTime,
                StdSetupTime = x.StdSetupTime,
                SetupConflictRatio = x.SetupConflictRatio,
                SameMaterialRatio = x.SameMaterialRatio,
                SameDiameterRatio = x.SameDiameterRatio,
                MachineSpeedStd = x.MachineSpeedStd,
                MachineEfficiencyStd = x.MachineEfficiencyStd,
                MachineCapabilityImbalance = x.MachineCapabilityImbalance,
                AssignmentSensitivity = x.AssignmentSensitivity,
                MachinePreferenceSharpness = x.MachinePreferenceSharpness,
                BottleneckLoadRatio = x.BottleneckLoadRatio,
                MachineLoadStd = x.MachineLoadStd,
                BottleneckSensitivity = x.BottleneckSensitivity,
                TightDueDateRatio = x.TightDueDateRatio,
                AvgSlackTime = x.AvgSlackTime,
                CriticalJobRatio = x.CriticalJobRatio,
                InitialMakespan = x.InitialMakespan,
                InitialObjective = x.InitialObjective,
                Neighbors = x.Neighbors,
                Tenure = x.Tenure,
                RelativeObjective = x.RelativeObjective
            };
        }
    }

    public class ObjectivePrediction
    {
        [ColumnName("Score")]
        public float Score { get; set; }
    }
}
