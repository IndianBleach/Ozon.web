using Storage.Api.ClickHouse;

namespace Storage.Api.Extensions
{
    internal static class ClickHouseStorageExtensions
    {
        internal static void AddClickHouseStorageClient(
            this IServiceCollection services,
            string ch_connectionString,
            ClickHouseStorageServiceOptions chOptions)
        {
            services.AddSingleton<IClickHouseStorageClient, ClickHouseStorageClient>(
                serviceProvider => {
                    return new ClickHouseStorageClient(
                        logger: serviceProvider.GetRequiredService<ILogger<ClickHouseStorageClient>>(),
                        options: chOptions,
                        connectionString: ch_connectionString);
                });
        }
    }
}
