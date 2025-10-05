namespace Showcase.DistributedLock.Services.v2
{
    public interface INightlyReportService
    {
        Task ProcessAsync(CancellationToken cancellationToken);
    }
}