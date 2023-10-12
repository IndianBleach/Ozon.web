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

            factory.Register<string, MarketplaceProductStorageRegistrationRead>(new ConsumerConfig()
            {
                GroupId = nameof(MarketplaceProductStorageRegistrationRead),
                AutoOffsetReset = AutoOffsetReset.Latest,
                BootstrapServers = kafkaHost,
                EnableAutoCommit = false,
                AutoCommitIntervalMs = 0,
                Acks = Acks.Leader
            });

            factory.Register<string, SyncProductRegistryInfoRequest>(new ConsumerConfig()
            {
                GroupId = nameof(SyncProductRegistryInfoRequest),
                AutoOffsetReset = AutoOffsetReset.Latest,
                BootstrapServers = kafkaHost,
                EnableAutoCommit = false,
                AutoCommitIntervalMs = 0,
                Acks = Acks.Leader
            });

            //SyncProductRegistryInfoAnswer

            services.AddSingleton<IConsumerFactory>(factory);

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
