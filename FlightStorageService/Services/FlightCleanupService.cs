namespace FlightStorageService.Services
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FlightStorageService.Repository;

    public class FlightCleanupService : BackgroundService
    {
        private readonly ILogger<FlightCleanupService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public FlightCleanupService(ILogger<FlightCleanupService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("The flight cleaning service has been launched.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<FlightRepository>();

                    int deleted = await repository.CleanupOldFlightsAsync();

                    if (deleted > 0)
                        _logger.LogInformation("✅ Cleaning performed automatically. {Count} old flights deleted.", deleted);
                    else
                        _logger.LogInformation("Cleaning performed automatically. No old flights found.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during automatic flight cleanup.");
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // 1 раз в день
            }
        }
    }
}
