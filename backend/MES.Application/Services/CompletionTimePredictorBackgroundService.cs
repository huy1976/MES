using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MesMicroservice.Application.Services
{
    public class CompletionTimePredictorBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Timer timer;

        public CompletionTimePredictorBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            timer = new Timer(60000);

            timer.Elapsed += TimerTick;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer.Enabled = true;

            return Task.CompletedTask;
        }

        private async void TimerTick(object? sender, ElapsedEventArgs e)
        {
            using var scope = _serviceScopeFactory.CreateAsyncScope();

            var predictor = scope.ServiceProvider.GetRequiredService<CompletionTimePredictor>();

            await predictor.UpdatePrediction();
        }
    }

}
