namespace Showcase.DistributedLock.Repositories
{
    public interface IPostgresRepository
    {
        Task<bool> TryExecuteCommandAsync(long key);

        Task<bool> ExecuteCommandAsync();
    }
}