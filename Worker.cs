using Showcase.DistributedLock.Services.v2;

namespace Showcase.DistributedLock
{
    public class Worker(ILogger<Worker> logger, IServiceProvider serviceProvider) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<INightlyReportService>();

            while (!cancellationToken.IsCancellationRequested)
            {
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await service.ProcessAsync(cancellationToken);
            }
        }
    }
}