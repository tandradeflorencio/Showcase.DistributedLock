using Medallion.Threading;
using Showcase.DistributedLock.Repositories;

namespace Showcase.DistributedLock.Services.v2
{
    public class NightlyReportService(ILogger<NightlyReportService> logger, IDistributedLockProvider distributedLockProvider, IPostgresRepository postgresRepository) : INightlyReportService
    {
        public const string LockKey = "nightly-report";
        public static TimeSpan LockTimeout => TimeSpan.FromSeconds(3);

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            await using var distributedLock = await distributedLockProvider.TryAcquireLockAsync(LockKey, LockTimeout, cancellationToken);

            if (distributedLock is null)
            {
                logger.LogInformation("An another instance is already processing the report. This one will be canceled.");
                return;
            }

            await postgresRepository.ExecuteCommandAsync();

            Console.Write("Report processed.");
        }
    }
}