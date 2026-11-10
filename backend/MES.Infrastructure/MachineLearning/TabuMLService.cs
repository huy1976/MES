using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;

public interface ITabuMLService
{
    void TrainModel();
    (int Tenure, int Neighbors) Predict(TabuDataInput input);
}

public class TabuMLService : ITabuMLService
{
    private readonly MLContext _mlContext;
    private ITransformer? _tenureModel;
    private ITransformer? _neighborsModel;

    private PredictionEngine<TabuDataInput, TabuPrediction>? _tenureEngine;
    private PredictionEngine<TabuDataInput, TabuPrediction>? _neighborsEngine;

    private readonly string _dataPath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Data",
        "Tabu_ML_Training_Data_Averaged.csv");

    public TabuMLService()
    {
        _mlContext = new MLContext(seed: 0);
    }

    public void TrainModel()
    {
        if (!File.Exists(_dataPath))
            throw new FileNotFoundException($"Không tìm thấy file train data: {_dataPath}");

        IDataView dataView =
            _mlContext.Data.LoadFromTextFile<TabuDataInput>(
                _dataPath,
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
            _mlContext.Transforms.Concatenate("Features", featureColumns)
            .Append(_mlContext.Regression.Trainers.FastTree(
                labelColumnName: nameof(TabuDataInput.Tenure),
                featureColumnName: "Features"));

        var neighborsPipeline =
            _mlContext.Transforms.Concatenate("Features", featureColumns)
            .Append(_mlContext.Regression.Trainers.FastTree(
                labelColumnName: nameof(TabuDataInput.Neighbors),
                featureColumnName: "Features"));

        _tenureModel = tenurePipeline.Fit(dataView);
        _neighborsModel = neighborsPipeline.Fit(dataView);

        _tenureEngine = _mlContext.Model.CreatePredictionEngine<TabuDataInput, TabuPrediction>(_tenureModel);
        _neighborsEngine = _mlContext.Model.CreatePredictionEngine<TabuDataInput, TabuPrediction>(_neighborsModel);
    }

    public (int Tenure, int Neighbors) Predict(TabuDataInput input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        if (_tenureEngine == null || _neighborsEngine == null)
            throw new InvalidOperationException("Model chưa được train. Hãy gọi TrainModel() trước.");

        var tenureScore = _tenureEngine.Predict(input).PredictedValue;
        var neighborsScore = _neighborsEngine.Predict(input).PredictedValue;

        return (
            (int)Math.Round(tenureScore),
            (int)Math.Round(neighborsScore)
        );
    }
}

public class TenurePredictionOutput
{
    [ColumnName("Score")]
    public float PredictedValue { get; set; }
}