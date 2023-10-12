using Confluent.Kafka;
using Marketplace.Api.Kafka;
using Ozon.Bus.DTOs.ProductsRegistry;
using Ozon.Bus.DTOs.StorageService;
using Ozon.Bus;

namespace Marketplace.Api.Extensions
{
    public static class ConsumerFactoryExtensions
    {
        public static void AddConsumerFactory(
            this IServiceCollection services,
            string kafkaHost)
        {
            var factory = new ConsumerFactory();

            factory.Register<string, ProductRegistryMarketplaceSeller>(new ConsumerConfig()
            {
                GroupId = nameof(ProductRegistryMarketplaceSeller),
                AutoOffsetReset = AutoOffsetReset.Latest,
                BootstrapServers = kafkaHost,
                EnableAutoCommit = false,
                AutoCommitIntervalMs = 0,
            });

            factory.Register<string, AddStorageMessage>(new ConsumerConfig()
            {
                GroupId = nameof(AddStorageMessage),
                AutoOffsetReset = AutoOffsetReset.Latest,
                BootstrapServers = kafkaHost,
                EnableAutoCommit = false,
                AutoCommitIntervalMs = 0,
            });

            factory.Register<string, SyncProductRegistryInfoAnswer>(new ConsumerConfig()
            {
                GroupId = nameof(SyncProductRegistryInfoAnswer),
                AutoOffsetReset = AutoOffsetReset.Latest,
                BootstrapServers = kafkaHost,
                EnableAutoCommit = false,
                AutoCommitIntervalMs = 0,
            });

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
