using Showcase.DistributedLock.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace Showcase.DistributedLock.Services.v1
{
    public class NightlyReportService(ILogger<NightlyReportService> logger, IPostgresRepository postgresRepository) : INightlyReportService
    {
        public const string LockKey = "nightly-report";

        public async Task Process(CancellationToken cancellationToken)
        {
            var key = GenerateInt64HashedKey(LockKey);

            try
            {
                var aquired = await postgresRepository.TryExecuteCommandAsync(key);

                if (!aquired)
                {
                    logger.LogInformation("An another instance is already processing the report. This one will be canceled.");
                    return;
                }

                Console.Write("Report processed.");
            }
            finally
            {
                await postgresRepository.TryExecuteCommandAsync(key);
                await Task.Delay(5000, cancellationToken);
            }
        }

        private static long GenerateInt64HashedKey(string key) =>
            BitConverter.ToInt64(SHA256.HashData(Encoding.UTF8.GetBytes(key)), 0);
    }
}