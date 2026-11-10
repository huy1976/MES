using Microsoft.ML;
using MesMicroservice.MachineLearning;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MesMicroservice.Api.BackgroundServices
{
    /// <summary>
    /// Dịch vụ nền tự động train lại mô hình Tabu AI từ file feedback
    /// </summary>
    public class AITabuModelRetrainService : BackgroundService
    {
        private readonly ILogger<AITabuModelRetrainService> _logger;
        private readonly string _dataPath;
        private readonly string _baseDir;
        private readonly TimeSpan _retrainInterval = TimeSpan.FromHours(12); // chạy lại mỗi 12h
        private readonly MLContext _mlContext;

        public AITabuModelRetrainService(ILogger<AITabuModelRetrainService> logger)
        {
            _logger = logger;
            _baseDir = AppContext.BaseDirectory;
            _dataPath = Path.Combine(_baseDir, "Tabu_Data_Ultimate.csv");
            _mlContext = new MLContext(seed: 42);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[AI Retrain] Background retrain service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (File.Exists(_dataPath))
                    {
                        _logger.LogInformation($"[AI Retrain] Starting retrain using {_dataPath}");
                        RetrainModels();
                    }
                    else
                    {
                        _logger.LogWarning($"[AI Retrain] Feedback file not found: {_dataPath}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[AI Retrain] Retrain failed");
                }

                await Task.Delay(_retrainInterval, stoppingToken);
            }

            _logger.LogInformation("[AI Retrain] Background retrain service stopped.");
        }

        private void RetrainModels()
        {
            IDataView data = _mlContext.Data.LoadFromTextFile<TabuAITrainingData>(
                path: _dataPath, hasHeader: true, separatorChar: ',');

            // tạo cột feature
            var featureCols = typeof(TabuAITrainingData)
                .GetProperties()
                .Where(p => p.Name != "Neighbors" && p.Name != "Tenure")
                .Select(p => p.Name)
                .ToArray();

            var pipeline = _mlContext.Transforms
                .Concatenate("Features", featureCols)
                .Append(_mlContext.Transforms.NormalizeMinMax("Features"));

            string timeMark = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            // Train Neighbors
            var neighborPipeline = pipeline.Append(
                _mlContext.Regression.Trainers.LightGbm("Neighbors", "Features", learningRate: 0.1));

            var neighborModel = neighborPipeline.Fit(data);
            string neighborModelPath = Path.Combine(_baseDir, $"NeighborModel_{timeMark}.zip");
            _mlContext.Model.Save(neighborModel, data.Schema, neighborModelPath);
            _logger.LogInformation($"[AI Retrain] Saved {neighborModelPath}");

            // Train Tenure
            var tenurePipeline = pipeline.Append(
                _mlContext.Regression.Trainers.LightGbm("Tenure", "Features", learningRate: 0.1));

            var tenureModel = tenurePipeline.Fit(data);
            string tenureModelPath = Path.Combine(_baseDir, $"TenureModel_{timeMark}.zip");
            _mlContext.Model.Save(tenureModel, data.Schema, tenureModelPath);
            _logger.LogInformation($"[AI Retrain] Saved {tenureModelPath}");
        }
    }
}
