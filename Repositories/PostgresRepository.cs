using Dapper;
using Npgsql;

namespace Showcase.DistributedLock.Repositories
{
    public class PostgresRepository(NpgsqlDataSource dataSource) : IPostgresRepository
    {
        public async Task<bool> TryExecuteCommandAsync(long key)
        {
            using var connection = dataSource.OpenConnection();
            var aquired = false;

            try
            {
                aquired = await TryAdvisoryLockAsync(connection, key);

                if (!aquired)
                    return false;

                await ExecuteCommandAsync(connection);
            }
            finally
            {
                await TryAdvisoryUnlockAsync(connection, key);
            }

            return aquired;
        }

        public async Task<bool> ExecuteCommandAsync()
        {
            const string query = @"Select count(0) from ""ToDo""";

            using var connection = dataSource.OpenConnection();

            _ = await connection.ExecuteScalarAsync<int>(query);

            return true;
        }

        private static async Task ExecuteCommandAsync(NpgsqlConnection connection)
        {
            const string query = @"Select count(0) from ""ToDo""";

            _ = await connection.ExecuteScalarAsync<int>(query);
        }

        private static async Task<bool> TryAdvisoryLockAsync(NpgsqlConnection connection, long key)
        {
            const string query = "Select pg_try_advisory_lock(@key)";

            var locked = await connection.ExecuteScalarAsync<bool>(query, new { key });

            return locked;
        }

        private static async Task<bool> TryAdvisoryUnlockAsync(NpgsqlConnection connection, long key)
        {
            const string query = "Select pg_advisory_unlock(@key)";

            var unlocked = await connection.ExecuteScalarAsync<bool>(query, new { key });

            return unlocked;
        }
    }
}