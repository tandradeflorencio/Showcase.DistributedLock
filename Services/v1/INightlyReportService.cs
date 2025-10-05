namespace Showcase.DistributedLock.Services.v1
{
    public interface INightlyReportService
    {
        Task Process(CancellationToken cancellationToken);
    }
}