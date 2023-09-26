using Confluent.Kafka;
using Ozon.Bus.DTOs.ProductsRegistry;
using Ozon.Bus.DTOs.StorageService;
using Ozon.Bus;

namespace Storage.Api.Extensions
{
    public static class ConsumerFactoryExtensions
    {
        public static void AddConsumerFactory(
            this IServiceCollection services,
            string kafkaHost)
        {
            var factory = new ConsumerFactory();

            factory.Register<string, StorageProductUpdateMarketplaceStockInfo>(new ConsumerConfig()
            {
                GroupId = nameof(StorageProductUpdateMarketplaceStockInfo),
                AutoOffsetReset = AutoOffsetReset.Latest,
                BootstrapServers = kafkaHost,
                EnableAutoCommit = false,
                AutoCommitIntervalMs = 0,
            });

            services.AddSingleton<IConsumerFactory>(factory);
        }
    }
}
