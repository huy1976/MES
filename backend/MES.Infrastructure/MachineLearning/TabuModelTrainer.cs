using Microsoft.ML;
using System;
using System.IO;

public class TabuModelTrainer
{
    public void TrainAndSaveModels(string csvPath, string outputTenureZip, string outputNeighborsZip)
    {
        var mlContext = new MLContext(seed: 0);

        IDataView dataView =
            mlContext.Data.LoadFromTextFile<TabuDataInput>(
                csvPath,
                hasHeader: true,
                separatorChar: ',');

        var featureColumns = new[]
        {
            nameof(TabuDataInput.TotalJobs),
            nameof(TabuDataInput.MachineCount),
            nameof(TabuDataInput.AvgEligibleMachines),
            nameof(TabuDataInput.MinEligibleMachines),
            nameof(TabuDataInput.MaxEligibleMachines),
            nameof(TabuDataInput.AvgProcessingTime),
            nameof(TabuDataInput.StdProcessingTime),
            nameof(TabuDataInput.AvgDiameter),
            nameof(TabuDataInput.DiameterVariance),
            nameof(TabuDataInput.AvgSetupTime),
            nameof(TabuDataInput.StdSetupTime),
            nameof(TabuDataInput.SetupConflictRatio),
            nameof(TabuDataInput.SameMaterialRatio),
            nameof(TabuDataInput.SameDiameterRatio),
            nameof(TabuDataInput.MachineSpeedStd),
            nameof(TabuDataInput.MachineEfficiencyStd),
            nameof(TabuDataInput.MachineCapabilityImbalance),
            nameof(TabuDataInput.AssignmentSensitivity),
            nameof(TabuDataInput.MachinePreferenceSharpness),
            nameof(TabuDataInput.BottleneckLoadRatio),
            nameof(TabuDataInput.MachineLoadStd),
            nameof(TabuDataInput.BottleneckSensitivity),
            nameof(TabuDataInput.TightDueDateRatio),
            nameof(TabuDataInput.AvgSlackTime),
            nameof(TabuDataInput.CriticalJobRatio),
            nameof(TabuDataInput.InitialMakespan),
            nameof(TabuDataInput.InitialObjective)
        };

        var tenurePipeline =
            mlContext.Transforms.CopyColumns("Label", nameof(TabuDataInput.Tenure))
                .Append(mlContext.Transforms.Concatenate("Features", featureColumns))
                .Append(mlContext.Regression.Trainers.FastTree());

        var neighborsPipeline =
            mlContext.Transforms.CopyColumns("Label", nameof(TabuDataInput.Neighbors))
                .Append(mlContext.Transforms.Concatenate("Features", featureColumns))
                .Append(mlContext.Regression.Trainers.FastTree());

        var tenureModel = tenurePipeline.Fit(dataView);
        var neighborsModel = neighborsPipeline.Fit(dataView);

        mlContext.Model.Save(tenureModel, dataView.Schema, outputTenureZip);
        mlContext.Model.Save(neighborsModel, dataView.Schema, outputNeighborsZip);

        Console.WriteLine("Đã huấn luyện và lưu file .zip thành công!");
    }
}