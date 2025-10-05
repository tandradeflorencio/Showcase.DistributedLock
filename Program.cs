using Medallion.Threading;
using Medallion.Threading.Postgres;
using Serilog;
using Showcase.DistributedLock;
using Showcase.DistributedLock.Repositories;
using Showcase.DistributedLock.Services.v1;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting worker service...");

    var builder = Host.CreateApplicationBuilder(args);

    AddLogging(builder);

    builder.Services.AddHostedService<Worker>();

    AddDatabases(builder);

    AddRepositories(builder);

    AddServices(builder);

    var host = builder.Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker service terminated unexpectedly.");
}
finally
{
    await Log.CloseAndFlushAsync();
}

static void AddServices(HostApplicationBuilder builder)
{
    builder.Services.AddScoped<INightlyReportService, NightlyReportService>();
    builder.Services.AddScoped<Showcase.DistributedLock.Services.v2.INightlyReportService, Showcase.DistributedLock.Services.v2.NightlyReportService>();
}

static void AddRepositories(HostApplicationBuilder builder)
{
    builder.Services.AddScoped<IPostgresRepository, PostgresRepository>();
}

static void AddDatabases(HostApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("Postgres");

    builder.Services.AddSingleton<IDistributedLockProvider>(
        _ =>
        {
            return new PostgresDistributedSynchronizationProvider(connectionString!);
        });

    builder.Services.AddNpgsqlDataSource(connectionString!);
}

static void AddLogging(HostApplicationBuilder builder)
{
    builder.Logging.ClearProviders();
    Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .CreateLogger();

    builder.Logging.AddSerilog();
}