using Microsoft.ML;
using MesMicroservice.MachineLearning;
using System;
using System.IO;
using System.Linq;

namespace MesMicroservice.MLTrainer
{
    internal class DataProcessor
    {
        private readonly MLContext _mlContext;

        public DataProcessor()
        {
            _mlContext = new MLContext(seed: 42);
        }

        public void TrainAndSaveModels(string dataPath = "Tabu_Data_Ultimate.csv")
        {
            if (!File.Exists(dataPath))
            {
                Console.WriteLine($"❌ Không tìm thấy file dữ liệu: {dataPath}");
                return;
            }

            IDataView rawData = _mlContext.Data.LoadFromTextFile<TabuAITrainingData>(
                path: dataPath, hasHeader: true, separatorChar: ',');

            var allRows = _mlContext.Data
                .CreateEnumerable<TabuAITrainingData>(rawData, reuseRowObject: false)
                .ToList();

            // ==========================================================
            // 🔹 1. Sinh dữ liệu nhân tạo (Augmentation)
            // ==========================================================
            var augmented = new List<TabuAITrainingData>(allRows);

            Random rnd = new Random(42);
            foreach (var r in allRows)
            {
                // sao chép một vài biến thể nhẹ của cùng bản ghi
                for (int i = 0; i < 3; i++)
                {
                    var clone = new TabuAITrainingData
                    {
                        TotalJobs = r.TotalJobs,
                        MachineCount = r.MachineCount,
                        AvgEligibleMachines = r.AvgEligibleMachines,
                        MinEligibleMachines = r.MinEligibleMachines,
                        MaxEligibleMachines = r.MaxEligibleMachines,
                        AvgProcessingTime = r.AvgProcessingTime,
                        StdProcessingTime = r.StdProcessingTime,
                        AvgDiameter = r.AvgDiameter,
                        DiameterVariance = r.DiameterVariance,
                        AvgSetupTime = r.AvgSetupTime,
                        StdSetupTime = r.StdSetupTime,
                        SetupConflictRatio = r.SetupConflictRatio,
                        SameMaterialRatio = r.SameMaterialRatio,
                        SameDiameterRatio = r.SameDiameterRatio,
                        MachineSpeedStd = r.MachineSpeedStd,
                        MachineEfficiencyStd = r.MachineEfficiencyStd,
                        MachineCapabilityImbalance = r.MachineCapabilityImbalance,
                        AssignmentSensitivity = r.AssignmentSensitivity,
                        MachinePreferenceSharpness = r.MachinePreferenceSharpness,
                        BottleneckLoadRatio = r.BottleneckLoadRatio,
                        MachineLoadStd = r.MachineLoadStd,
                        BottleneckSensitivity = r.BottleneckSensitivity,
                        TightDueDateRatio = r.TightDueDateRatio,
                        AvgSlackTime = r.AvgSlackTime,
                        CriticalJobRatio = r.CriticalJobRatio,
                        InitialMakespan = r.InitialMakespan,
                        InitialObjective = r.InitialObjective
                    };

                    // tạo phần tử ngẫu nhiên ±20% quanh Tenure/N thật
                    float scaleT = (float)(0.7 + rnd.NextDouble() * 0.6); // 0.7–1.3
                    float scaleN = (float)(0.6 + rnd.NextDouble() * 0.8); // 0.6–1.4

                    float newT = Math.Clamp(r.Tenure * scaleT, 5, 50);
                    float newN = Math.Clamp(r.Neighbors * scaleN, 10, 200);

                    clone.Tenure = newT;
                    clone.Neighbors = newN;

                    // Objective hơi dao động (noise)
                    clone.InitialObjective = (float)(r.InitialObjective * (0.95 + rnd.NextDouble() * 0.1));

                    augmented.Add(clone);
                }
            }

            Console.WriteLine($"📈 Dữ liệu gốc: {allRows.Count} | sau nhân tạo: {augmented.Count}");

            var data = _mlContext.Data.LoadFromEnumerable(augmented);

            // ==========================================================
            // 🔹 2. Huấn luyện lại như cũ (LightGBM)
            // ==========================================================
            var featureColumns = new[]
            {
        nameof(TabuAITrainingData.TotalJobs),
        nameof(TabuAITrainingData.MachineCount),
        nameof(TabuAITrainingData.AvgEligibleMachines),
        nameof(TabuAITrainingData.MinEligibleMachines),
        nameof(TabuAITrainingData.MaxEligibleMachines),
        nameof(TabuAITrainingData.AvgProcessingTime),
        nameof(TabuAITrainingData.StdProcessingTime),
        nameof(TabuAITrainingData.AvgDiameter),
        nameof(TabuAITrainingData.DiameterVariance),
        nameof(TabuAITrainingData.AvgSetupTime),
        nameof(TabuAITrainingData.StdSetupTime),
        nameof(TabuAITrainingData.SetupConflictRatio),
        nameof(TabuAITrainingData.SameMaterialRatio),
        nameof(TabuAITrainingData.SameDiameterRatio),
        nameof(TabuAITrainingData.MachineSpeedStd),
        nameof(TabuAITrainingData.MachineEfficiencyStd),
        nameof(TabuAITrainingData.MachineCapabilityImbalance),
        nameof(TabuAITrainingData.AssignmentSensitivity),
        nameof(TabuAITrainingData.MachinePreferenceSharpness),
        nameof(TabuAITrainingData.BottleneckLoadRatio),
        nameof(TabuAITrainingData.MachineLoadStd),
        nameof(TabuAITrainingData.BottleneckSensitivity),
        nameof(TabuAITrainingData.TightDueDateRatio),
        nameof(TabuAITrainingData.AvgSlackTime),
        nameof(TabuAITrainingData.CriticalJobRatio),
        nameof(TabuAITrainingData.InitialMakespan),
        nameof(TabuAITrainingData.InitialObjective)
    };

            var pipelineBase = _mlContext.Transforms
                .Concatenate("Features", featureColumns)
                .Append(_mlContext.Transforms.NormalizeMinMax("Features"));

            string timeMark = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            var neighborPipeline = pipelineBase.Append(
                _mlContext.Regression.Trainers.LightGbm("Neighbors", "Features", learningRate: 0.1));

            var neighborModel = neighborPipeline.Fit(data);
            _mlContext.Model.Save(neighborModel, data.Schema, $"NeighborModel_{timeMark}.zip");
            Console.WriteLine($"✅ Train xong NeighborModel_{timeMark}.zip");

            var tenurePipeline = pipelineBase.Append(
                _mlContext.Regression.Trainers.LightGbm("Tenure", "Features", learningRate: 0.1));

            var tenureModel = tenurePipeline.Fit(data);
            _mlContext.Model.Save(tenureModel, data.Schema, $"TenureModel_{timeMark}.zip");
            Console.WriteLine($"✅ Train xong TenureModel_{timeMark}.zip");

            Console.WriteLine("🎯 Huấn luyện hoàn tất và đã thêm dữ liệu nhân tạo đa dạng!");
        }

    }
}
