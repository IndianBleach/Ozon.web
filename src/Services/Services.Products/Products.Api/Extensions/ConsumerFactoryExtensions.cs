using Confluent.Kafka;
using Ozon.Bus.DTOs.StorageService;
using Ozon.Bus;

namespace Products.Api.Extensions
{
    public static class ConsumerFactoryExtensions
    {
        public static void AddConsumerFactory(
            this IServiceCollection services,
            string kafkaHost)
        {
            var factory = new ConsumerFactory();

            factory.Register<string, SyncProductRegistryInfoRequest>(new ConsumerConfig()
            {
                GroupId = nameof(SyncProductRegistryInfoRequest),
                AutoOffsetReset = AutoOffsetReset.Latest,
                BootstrapServers = kafkaHost,
                EnableAutoCommit = false,
                AutoCommitIntervalMs = 0,
                Acks = Acks.Leader
            });

            services.AddSingleton<IConsumerFactory>(factory);
        }
    }
}
