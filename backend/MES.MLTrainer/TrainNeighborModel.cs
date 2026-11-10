using Microsoft.ML;
using MesMicroservice.MachineLearning;
using System;

namespace MesMicroservice.MLTrainer
{
    internal class ProgramTrain
    {
        static void Main(string[] args)
        {
            var mlContext = new MLContext();

            // Load file dữ liệu 1500 dòng mới nhất
            IDataView data = mlContext.Data.LoadFromTextFile<TabuAITrainingData>(
                    path: "Tabu_Data_Synthetic_v1.csv",
                    hasHeader: true,
                    separatorChar: ',');

            // 1. TẠO CỘT FEATURES CHUNG
            var dataProcessPipeline = mlContext.Transforms.Concatenate(
                "Features",
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
                nameof(TabuAITrainingData.InitialObjective),
                nameof(TabuAITrainingData.Neighbors),
                nameof(TabuAITrainingData.Tenure)


            );

            // ==============================================
            // 2. TRAIN NEIGHBOR MODEL (LƯU Ý TÊN CỘT LÀ "Neighbors")
            // ==============================================
            var objectivePipeline = dataProcessPipeline.Append(
                mlContext.Regression.Trainers.FastTree(
                    labelColumnName: nameof(TabuAITrainingData.RelativeObjective),
                    featureColumnName: "Features"));

            var objectiveModel = objectivePipeline.Fit(data);
            mlContext.Model.Save(objectiveModel, data.Schema, "TabuObjectiveModel.zip");

            Console.WriteLine("Train xong TabuObjectiveModel.zip");

            // ==============================================
            // 3. TRAIN TENURE MODEL (LƯU Ý TÊN CỘT LÀ "Tenure")
            // ==============================================
            var tenurePipeline = dataProcessPipeline.Append(
                mlContext.Regression.Trainers.FastTree(
                    labelColumnName: "Tenure", // 👈 CHỮ CHUẨN TRONG FILE CSV MỚI
                    featureColumnName: "Features"));

            var tenureModel = tenurePipeline.Fit(data);
            mlContext.Model.Save(tenureModel, data.Schema, "TenureModel.zip");
            Console.WriteLine("✅ Train xong TenureModel.zip");

            Console.WriteLine("\n🎉 Hoàn tất! Hãy copy 2 file .zip mới đè vào dự án của bạn.");
        }
    }
}